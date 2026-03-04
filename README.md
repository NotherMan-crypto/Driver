# Hệ Thống Ôn Thi Giấy Phép Lái Xe (Driver Prep 2026)

Chào mừng bạn đến với dự án **Hệ Thống Ôn Thi Giấy Phép Lái Xe**. Đây là một hệ sinh thái toàn diện bao gồm cả ứng dụng **Web (Next.js)** và ứng dụng **Đa nền tảng (MAUI)**, được xây dựng để giúp học viên ôn thuy lý thuyết và thi sa hình giao thông Việt Nam một cách hiệu quả nhất.

## Tính Năng Nổi Bật (Features)
- **Giao diện đẳng cấp (Premium UI/UX):** Giao diện Dark Theme hiện đại, hình ảnh trực quan sinh động, tối ưu hóa hiển thị (Responsive).
- **Hệ Thống Học Trực Tuyến:** Ngân hàng hơn 600 câu hỏi độc quyền (A1, A2, A3, B1, B2...), được tổ chức theo từng cấu trúc đề thi.
- **Xác thực bảo mật:** Tích hợp bảo mật Firebase Identity/Auth (Google Sign-In, Email/Password).
- **Phân Lớp Quản Trị (Admin Dashboard):** Phân quyền vai trò để giám sát bộ câu hỏi và lượng truy cập.
- **Micro-animations & Hiệu ứng:** Cải thiện trải nghiệm thao tác của người dùng bằng Framer Motion (Glassmorphism, Neon glow).

## Công Nghệ Sử Dụng (Tech Stack)
### Web (Frontend & Backend)
- **Framework:** Next.js 14 (App Router)
- **Ngôn ngữ:** TypeScript
- **Styling:** Tailwind CSS + Framer Motion
- **Database/Auth:** Google Firebase (Firestore + Authentication)

### Mobile & Desktop App (Cross-Platform)
- **Framework:** .NET 8 MAUI (Multi-platform App UI)
- **Ngôn ngữ:** C# & XAML
- **Target:** Android, iOS, Windows, MacCatalyst

---

## Hướng Dẫn Triển Khai (Deployment)
Dự án được kết cấu để dễ dàng triển khai lên môi trường Production (hiện đang được thiết lập với Next.js Export trên máy chủ **Ubuntu VPS**).

1. Hệ thống sử dụng PM2 Process Manager trên cổng mặc định (HTTP port 80).
2. Firebase Admin SDK được cấu hình an toàn trên máy chủ.
3. Yêu cầu Node.js v20 LTS và NPM để khởi tạo môi trường Web.
4. Yêu cầu .NET 8 SDK & Visual Studio 2022 để khởi chạy dự án MAUI.

---

*Phát triển độc quyền bởi Không AI / NotherMan-crypto 2026.*
