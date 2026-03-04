import fs from 'fs';
import path from 'path';
import { QuestionBank, Question, LICENSE_TYPES } from './types';
import { db } from './firebase.config';
import { collection, getDocs, query, limit } from 'firebase/firestore';

// Cache the data in memory for performance (Server-side/Client-side)
let cachedData: QuestionBank | null = null;

export async function getQuestionBank(): Promise<QuestionBank> {
    // Return cached data if available
    if (cachedData && cachedData.Questions.length > 0) return cachedData;

    try {
        console.log("Fetching questions from Firestore...");
        const qRef = collection(db, 'questions');
        const querySnapshot = await getDocs(qRef);

        const mappedQuestions: Question[] = [];

        querySnapshot.forEach((doc) => {
            const result = doc.data();
            mappedQuestions.push({
                Id: result.Id,
                Text: result.Text,
                Answers: result.Answers,
                CorrectIndex: result.CorrectIndex,
                IsFatal: result.IsFatal,
                Category: result.Category,
                ImageName: result.ImageName,
                Explanation: result.Explanation
            });
        });

        // Fallback to local JSON if Firestore is empty (not seeded yet)
        if (mappedQuestions.length === 0) {
            console.warn("Firestore empty. Falling back to local JSON...");
            return await getLocalJSONFallback();
        }

        cachedData = {
            Version: "Cloud",
            Questions: mappedQuestions
        };

        return cachedData;
    } catch (error) {
        console.error('Error loading question bank from Firestore:', error);
        return await getLocalJSONFallback();
    }
}

async function getLocalJSONFallback(): Promise<QuestionBank> {
    try {
        const filePath = path.join(process.cwd(), 'public', 'data', 'question_bank.json');
        if (!fs.existsSync(filePath)) {
            return { Version: '0.0.0', Questions: [] };
        }
        const fileContent = fs.readFileSync(filePath, 'utf8');
        const rawData = JSON.parse(fileContent);

        let mappedQuestions: Question[] = [];
        if (rawData.Questions && Array.isArray(rawData.Questions)) {
            mappedQuestions = rawData.Questions.map((q: any) => ({
                Id: q.Id,
                Text: q.Text,
                Answers: q.Answers,
                CorrectIndex: q.CorrectIndex,
                IsFatal: q.IsFatal,
                Category: q.Category,
                ImageName: q.ImageName,
                Explanation: q.Explanation
            }));
        }
        return { Version: "Fallback", Questions: mappedQuestions };
    } catch (ex) {
        console.error(ex);
        return { Version: '0.0.0', Questions: [] };
    }
}

// Lấy ngẫu nhiên đề thi dựa theo hạng bằng
export async function getRandomExam(licenseId: string = 'B2'): Promise<Question[]> {
    const data = await getQuestionBank();

    // Tìm cấu hình luật thi cho bằng lái này
    const licenseConfig = LICENSE_TYPES.find(l => l.id === licenseId) || LICENSE_TYPES[5]; // Default B2

    // Chỗ này đáng lẽ cần logic trộn (bao nhiêu câu luật, cấu biển báo, câu điểm liệt...)
    // Nhưng trước mắt ta cứ bốc ngẫu nhiên N câu (trong đó đảm bảo có 1-2 câu điểm liệt nha)

    // 1. Lọc ra danh sách câu điểm liệt và không điểm liệt
    const fatalQuestions = data.Questions.filter(q => q.IsFatal);
    const normalQuestions = data.Questions.filter(q => !q.IsFatal);

    // 2. Trộn ngẫu nhiên cả hai
    const shuffledFatal = [...fatalQuestions].sort(() => 0.5 - Math.random());
    const shuffledNormal = [...normalQuestions].sort(() => 0.5 - Math.random());

    // 3. Chọn số lượng câu điểm liệt cần thiết (Giả sử 2 câu cho bằng nhỏ, 3-4 câu cho bằng lớn)
    let requiredFatalCount = 2; // Ví dụ
    if (licenseConfig.questionCount >= 35) requiredFatalCount = 3;
    if (licenseConfig.questionCount >= 45) requiredFatalCount = 4;

    const selectedFatal = shuffledFatal.slice(0, requiredFatalCount);
    const selectedNormal = shuffledNormal.slice(0, licenseConfig.questionCount - requiredFatalCount);

    // 4. Trộn kết quả cuối cùng lại một lần nữa
    const finalExam = [...selectedFatal, ...selectedNormal].sort(() => 0.5 - Math.random());

    return finalExam;
}
