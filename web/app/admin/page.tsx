"use client";

import { useAuth } from "@/components/AuthProvider";
import { seedDatabase } from "@/lib/seed";
import { useState } from "react";
import { ShieldAlert, Database, CheckCircle2, Loader2 } from "lucide-react";

export default function AdminDashboard() {
    const { user, role, loading } = useAuth();
    const [isSeeding, setIsSeeding] = useState(false);
    const [seedResult, setSeedResult] = useState<"idle" | "success" | "error">("idle");

    if (loading) {
        return <div className="min-h-screen flex items-center justify-center text-white">Đang xác thực quyền...</div>;
    }

    // Chặn user thường hoặc chưa đăng nhập
    if (!user || role !== "admin") {
        return (
            <div className="min-h-screen flex flex-col items-center justify-center p-4">
                <div className="glass max-w-md w-full p-8 rounded-2xl text-center space-y-4 border border-rose-500/20">
                    <div className="mx-auto w-16 h-16 bg-rose-500/10 rounded-full flex items-center justify-center text-rose-500 mb-6">
                        <ShieldAlert size={32} />
                    </div>
                    <h1 className="text-2xl font-bold text-white">Khu vực Cấm</h1>
                    <p className="text-slate-400">
                        Bạn cần đăng nhập bằng tài khoản Administrator để truy cập trang Quản trị này.
                    </p>
                </div>
            </div>
        );
    }

    const handleSeed = async () => {
        setIsSeeding(true);
        setSeedResult("idle");
        const success = await seedDatabase();
        setSeedResult(success ? "success" : "error");
        setIsSeeding(false);
    };

    return (
        <div className="min-h-screen bg-slate-950 p-6 md:p-12">
            <div className="max-w-6xl mx-auto space-y-8">
                <header className="flex flex-col md:flex-row gap-4 items-start md:items-center justify-between pb-6 border-b border-white/10">
                    <div>
                        <h1 className="text-3xl font-bold text-white mb-2">Dashboard Quản trị</h1>
                        <p className="text-slate-400">Chào mừng, {user.displayName}. Bạn đang truy cập với quyền cao nhất.</p>
                    </div>
                </header>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {/* System Sync Card */}
                    <div className="glass p-6 rounded-2xl border border-white/5 space-y-4 hover:border-indigo-500/30 transition-colors">
                        <div className="flex items-center gap-3">
                            <div className="p-3 bg-indigo-500/20 rounded-xl text-indigo-400">
                                <Database size={24} />
                            </div>
                            <div>
                                <h3 className="text-lg font-bold text-white">Đồng bộ Database</h3>
                                <p className="text-xs text-slate-400">Đẩy JSON file lên Firestore Cloud</p>
                            </div>
                        </div>

                        <p className="text-sm text-slate-400 pb-2">
                            Sử dụng chức năng này để ghi đè toàn bộ 600 câu hỏi từ JSON cục bộ lên cơ sở dữ liệu Firestore của Google.
                        </p>

                        <button
                            onClick={handleSeed}
                            disabled={isSeeding}
                            className="w-full flex items-center justify-center gap-2 py-3 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed text-white font-medium transition-all"
                        >
                            {isSeeding ? (
                                <>
                                    <Loader2 className="animate-spin" size={18} />
                                    Đang đồng bộ 600 câu...
                                </>
                            ) : seedResult === "success" ? (
                                <>
                                    <CheckCircle2 size={18} />
                                    Đã đồng bộ thành công
                                </>
                            ) : (
                                "Chạy lệnh Seed Database"
                            )}
                        </button>

                        {seedResult === "success" && (
                            <p className="text-xs text-emerald-400 text-center">Hoàn tất! Hệ thống hiện đã đọc từ Cloud Firestore.</p>
                        )}
                        {seedResult === "error" && (
                            <p className="text-xs text-rose-400 text-center">Lỗi cấu hình Firebase. Kiểm tra Console F12.</p>
                        )}
                    </div>

                    {/* Other features blocks */}
                    <div className="glass p-6 rounded-2xl border border-white/5 opacity-50 space-y-4">
                        <h3 className="text-lg font-bold text-white">Quản lý câu hỏi (Sắp ra mắt)</h3>
                        <p className="text-sm text-slate-400 pb-2">
                            Giao diện CRUD Thêm/Sửa/Xóa câu hỏi trực tiếp trên môi trường web. Đang được hoàn thiện.
                        </p>
                    </div>
                </div>

            </div>
        </div>
    );
}
