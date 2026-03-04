"use client";

import { useAuth } from "@/components/AuthProvider";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { Loader2 } from "lucide-react";

export function withAuth<P extends object>(WrappedComponent: React.ComponentType<P>) {
    return function WithAuth(props: P) {
        const { user, loading } = useAuth();
        const router = useRouter();

        useEffect(() => {
            // Nếu đã tải xong auth mà không có user -> Đá về trang đăng nhập
            if (!loading && !user) {
                router.replace("/auth");
            }
        }, [user, loading, router]);

        // Trong lúc đang kiểm tra Firebase Auth trên Client thì hiện màn hình chờ mờ ảo
        if (loading || !user) {
            return (
                <div className="min-h-screen bg-[#0a0a0a] flex flex-col items-center justify-center text-white">
                    <Loader2 className="w-12 h-12 text-indigo-500 animate-spin mb-4" />
                    <h2 className="text-xl font-light tracking-widest animate-pulse">
                        KIỂM TRA QUYỀN TRUY CẬP...
                    </h2>
                </div>
            );
        }

        // Nếu có user thì render component (Các trang thi)
        return <WrappedComponent {...props} />;
    };
}
