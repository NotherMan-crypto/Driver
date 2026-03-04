"use client";

import { useAuth } from "@/components/AuthProvider";
import { useState } from "react";
import { LogIn, UserPlus, Mail, Lock, User, Loader2, ArrowRight } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function AuthPage() {
    const { loginWithGoogle, loginWithEmail, registerWithEmail, user } = useAuth();
    const router = useRouter();
    const [isLogin, setIsLogin] = useState(true);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState("");

    // Form states
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [name, setName] = useState("");

    // Nếu đã đăng nhập, tự động về trang chủ
    if (user) {
        router.push("/");
        return null;
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setIsLoading(true);

        try {
            if (isLogin) {
                await loginWithEmail(email, password);
                router.push("/");
            } else {
                if (!name.trim()) throw new Error("Vui lòng nhập tên hiển thị");
                await registerWithEmail(email, password, name);
                router.push("/");
            }
        } catch (err: any) {
            let msg = "Đã có lỗi xảy ra.";
            if (err.code === "auth/email-already-in-use") msg = "Email này đã được đăng ký.";
            if (err.code === "auth/invalid-credential") msg = "Email hoặc mật khẩu không chính xác.";
            if (err.code === "auth/weak-password") msg = "Mật khẩu quá yếu (cần ít nhất 6 ký tự).";
            if (err.message) msg = err.message;
            setError(msg);
        } finally {
            setIsLoading(false);
        }
    };

    const handleGoogleLogin = async () => {
        setError("");
        setIsLoading(true);
        try {
            await loginWithGoogle();
            router.push("/");
        } catch (err) {
            setError("Lỗi đăng nhập qua Google.");
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-950 flex flex-col justify-center items-center p-4 relative overflow-hidden">
            {/* Background Effects */}
            <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-indigo-600/20 blur-[120px] rounded-full pointer-events-none"></div>
            <div className="absolute bottom-[-10%] right-[-10%] w-[30%] h-[30%] bg-pink-600/20 blur-[100px] rounded-full pointer-events-none"></div>

            <div className="w-full max-w-md">
                {/* Logo & Header */}
                <div className="text-center mb-8">
                    <Link href="/" className="inline-flex items-center justify-center p-3 glass rounded-2xl mb-4 border border-indigo-500/20 group hover:border-indigo-500/50 transition-all">
                        <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center shadow-lg shadow-indigo-500/20 group-hover:scale-105 transition-transform">
                            <span className="text-white font-black text-2xl tracking-tighter">A1</span>
                        </div>
                    </Link>
                    <h1 className="text-3xl font-bold text-white mb-2">
                        {isLogin ? "Chào mừng trở lại" : "Tạo tài khoản mới"}
                    </h1>
                    <p className="text-slate-400">
                        {isLogin ? "Đăng nhập để tiếp tục ôn luyện và lưu kết quả." : "Đăng ký ngay để mở khóa toàn bộ tính năng."}
                    </p>
                </div>

                {/* Main Card */}
                <div className="glass rounded-3xl p-6 md:p-8 border border-white/10 shadow-2xl relative z-10">

                    {/* Error Message */}
                    {error && (
                        <div className="mb-6 p-4 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-400 text-sm text-center animate-in fade-in slide-in-from-top-2">
                            {error}
                        </div>
                    )}

                    {/* Form */}
                    <form onSubmit={handleSubmit} className="space-y-4">
                        {!isLogin && (
                            <div className="space-y-1">
                                <label className="text-sm font-medium text-slate-300 ml-1">Tên hiển thị</label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-slate-500">
                                        <User className="h-5 w-5" />
                                    </div>
                                    <input
                                        type="text"
                                        required
                                        value={name}
                                        onChange={(e) => setName(e.target.value)}
                                        className="w-full bg-slate-900/50 border border-slate-800 text-white rounded-xl py-3 pl-12 pr-4 focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all outline-none placeholder:text-slate-600"
                                        placeholder="Nguyễn Văn A"
                                    />
                                </div>
                            </div>
                        )}

                        <div className="space-y-1">
                            <label className="text-sm font-medium text-slate-300 ml-1">Email</label>
                            <div className="relative">
                                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-slate-500">
                                    <Mail className="h-5 w-5" />
                                </div>
                                <input
                                    type="email"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    className="w-full bg-slate-900/50 border border-slate-800 text-white rounded-xl py-3 pl-12 pr-4 focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all outline-none placeholder:text-slate-600"
                                    placeholder="email@example.com"
                                />
                            </div>
                        </div>

                        <div className="space-y-1">
                            <label className="text-sm font-medium text-slate-300 ml-1">Mật khẩu</label>
                            <div className="relative">
                                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-slate-500">
                                    <Lock className="h-5 w-5" />
                                </div>
                                <input
                                    type="password"
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    className="w-full bg-slate-900/50 border border-slate-800 text-white rounded-xl py-3 pl-12 pr-4 focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all outline-none placeholder:text-slate-600"
                                    placeholder="••••••••"
                                />
                            </div>
                        </div>

                        <button
                            type="submit"
                            disabled={isLoading}
                            className="w-full mt-6 py-3.5 rounded-xl bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white font-bold shadow-lg shadow-indigo-500/25 flex items-center justify-center gap-2 transition-all disabled:opacity-70 disabled:cursor-not-allowed group"
                        >
                            {isLoading ? (
                                <Loader2 className="h-5 w-5 animate-spin" />
                            ) : (
                                <>
                                    {isLogin ? "Đăng nhập" : "Đăng ký tài khoản"}
                                    <ArrowRight className="h-5 w-5 group-hover:translate-x-1 transition-transform" />
                                </>
                            )}
                        </button>
                    </form>

                    <div className="mt-8 mb-6 relative">
                        <div className="absolute inset-0 flex items-center">
                            <div className="w-full border-t border-slate-800"></div>
                        </div>
                        <div className="relative flex justify-center text-sm">
                            <span className="px-4 bg-slate-900/80 text-slate-500 rounded-full text-xs uppercase tracking-widest font-bold">Hoặc tiếp tục với</span>
                        </div>
                    </div>

                    <button
                        type="button"
                        onClick={handleGoogleLogin}
                        disabled={isLoading}
                        className="w-full py-3.5 rounded-xl bg-white/5 hover:bg-white/10 border border-white/10 text-white font-medium flex items-center justify-center gap-3 transition-all disabled:opacity-50"
                    >
                        <Image src="https://www.svgrepo.com/show/475656/google-color.svg" alt="Google" width={24} height={24} className="opacity-90" />
                        Đăng nhập bằng Google
                    </button>

                    <div className="mt-8 text-center text-sm text-slate-400">
                        {isLogin ? "Chưa có tài khoản?" : "Đã có tài khoản?"}{" "}
                        <button
                            onClick={() => {
                                setIsLogin(!isLogin);
                                setError("");
                            }}
                            className="text-indigo-400 font-bold hover:text-indigo-300 transition-colors"
                        >
                            {isLogin ? "Đăng ký ngay" : "Đăng nhập"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
