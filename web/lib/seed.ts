import { db } from './firebase.config';
import { collection, writeBatch, doc } from 'firebase/firestore';
import questionBank from '../public/data/question_bank.json';

// Lấy danh sách câu hỏi
const questions = questionBank.Questions;

export async function seedDatabase() {
    try {
        console.log(`Bắt đầu đồng bộ ${questions.length} câu hỏi lên Firestore...`);

        // Firestore giới hạn 500 operations mỗi batch
        const batches = [];
        let currentBatch = writeBatch(db);
        let operationCount = 0;

        for (let i = 0; i < questions.length; i++) {
            const q = questions[i];

            // Dùng chính ID của câu hỏi làm ID Document cho dễ map
            const docRef = doc(collection(db, 'questions'), q.Id.toString());

            currentBatch.set(docRef, q);
            operationCount++;

            // Nếu đạt 400 operations thì push vào array và tạo batch mới
            if (operationCount === 400 || i === questions.length - 1) {
                batches.push(currentBatch.commit());
                currentBatch = writeBatch(db);
                operationCount = 0;
                console.log(`Đã đẩy lô ${batches.length}...`);
            }
        }

        await Promise.all(batches);
        console.log('✅ Đã đồng bộ toàn bộ 600 câu hỏi thành công!');
        return true;
    } catch (error) {
        console.error('Lỗi khi đồng bộ dữ liệu:', error);
        return false;
    }
}
