const { config } = require('dotenv');
const { resolve } = require('path');

// 1. Load env từ file .env.local TRƯỚC TIÊN
config({ path: resolve(process.cwd(), '.env.local') });

// 2. Import động thư viện SAU KHI env đã sẵn sàng để tránh biến rỗng
console.log("==========================================");
console.log("BẮT ĐẦU ĐẨY LẠI DATABASE 573 CÂU LÊN CLOUD");
console.log("==========================================");
console.log("Project ID đang kết nối:", process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID);

async function run() {
    const { seedDatabase } = await import('../lib/seed');
    try {
        const success = await seedDatabase();
        if (success) {
            console.log("✅ HOÀN TẤT SEED DATABASE THUẬN LỢI TỪ TERMINAL");
            process.exit(0);
        } else {
            console.error("❌ SEED BỊ LỖI QUYỀN TRUY CẬP FIRESTORE (Xem log lỗi phía trên)");
            process.exit(1);
        }
    } catch (err: any) {
        console.error("❌ CRASH KHÔNG AI NGỜ:", err);
        process.exit(1);
    }
}

run();
