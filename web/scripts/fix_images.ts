import fs from 'fs';
import path from 'path';

// Thư mục chứa ảnh và JSON
const imagesDir = path.join(process.cwd(), 'public', 'assets', 'images');
const jsonPath = path.join(process.cwd(), 'public', 'data', 'question_bank.json');

console.log("Bat dau fix ImageName cho question_bank.json...");

// Đọc toàn bộ tên file ảnh hiện có
let availableImages: string[] = [];
if (fs.existsSync(imagesDir)) {
    availableImages = fs.readdirSync(imagesDir);
} else {
    console.error("Khong tim thay thu muc anh:", imagesDir);
    process.exit(1);
}

// Đọc nội dung file JSON
if (!fs.existsSync(jsonPath)) {
    console.error("Khong tim thay file JSON:", jsonPath);
    process.exit(1);
}

const rawData = fs.readFileSync(jsonPath, 'utf8');
const questionBank = JSON.parse(rawData);

let fixCount = 0;
let removeCount = 0;
const validQuestions: any[] = [];

questionBank.Questions.forEach((q: any) => {
    // Nếu câu hỏi KHÔNG có yêu cầu hình ảnh -> là câu hợp lệ
    if (!q.ImageName || q.ImageName.trim() === "") {
        validQuestions.push(q);
        return;
    }

    // Nếu có yêu cầu hình, kiểm tra xem hình có tồn tại không
    const currentName = q.ImageName.trim();
    const baseNameWithoutExt = currentName.replace(/\.[^/.]+$/, "");
    let foundMatch = false;

    for (const imgFile of availableImages) {
        if (imgFile === currentName) {
            foundMatch = true;
            break;
        }

        const imgBaseNameWithoutExt = imgFile.replace(/\.[^/.]+$/, "");
        if (imgBaseNameWithoutExt === baseNameWithoutExt || imgBaseNameWithoutExt === `q${q.Id}`) {
            q.ImageName = imgFile;
            foundMatch = true;
            fixCount++;
            break;
        }
    }

    if (foundMatch) {
        validQuestions.push(q);
    } else {
        console.log(`[XÓA CÂU HỎI] Cau ${q.Id}: Không tìm thấy ảnh "${currentName}"`);
        removeCount++;
    }
});

questionBank.Questions = validQuestions;
fs.writeFileSync(jsonPath, JSON.stringify(questionBank, null, 2), 'utf8');

console.log(`✅ Hoan tat! Da fix Extension/Tên cho ${fixCount} anh. Đã xóa HOÀN TOÀN ${removeCount} câu hỏi do thiếu ảnh.`);
