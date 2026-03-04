"use client";

import { useAuth } from "@/components/AuthProvider";
import { LogOut, User as UserIcon, Shield, Loader2 } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { useState } from "react";

export function UserMenu() {
    const { user, role, loading, loginWithGoogle, logout } = useAuth();
    const [isOpen, setIsOpen] = useState(false);

    if (loading) {
        return (
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-white/5 border border-white/10">
                <Loader2 className="h-5 w-5 animate-spin text-slate-400" />
            </div>
        );
    }

    if (!user) {
        return (
            <Link
                href="/auth"
                className="flex items-center gap-2 rounded-full bg-indigo-500/20 px-4 py-2 border border-indigo-500/30 text-indigo-300 hover:bg-indigo-500 hover:text-white transition-all font-medium text-sm"
            >
                <UserIcon className="h-4 w-4" />
                Đăng nhập
            </Link>
        );
    }

    return (
        <div className="relative">
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center gap-2 rounded-full bg-white/5 p-1 pr-4 border border-white/10 hover:bg-white/10 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500/50"
            >
                {user.photoURL ? (
                    <Image
                        src={user.photoURL}
                        alt={user.displayName || "User"}
                        width={32}
                        height={32}
                        className="rounded-full bg-slate-800"
                    />
                ) : (
                    <div className="flex h-8 w-8 items-center justify-center rounded-full bg-indigo-500 text-white shadow-inner">
                        <UserIcon className="h-4 w-4" />
                    </div>
                )}
                <span className="text-sm font-medium text-slate-200 hidden sm:block">
                    {user.displayName?.split(" ")[0] || "User"}
                </span>
            </button>

            {isOpen && (
                <>
                    <div
                        className="fixed inset-0 z-40"
                        onClick={() => setIsOpen(false)}
                    ></div>
                    <div className="absolute right-0 top-12 z-50 w-64 rounded-xl border border-white/10 bg-slate-900/95 p-3 shadow-2xl backdrop-blur-xl animate-in fade-in slide-in-from-top-2 origin-top-right">
                        <div className="px-2 py-2 mb-2 border-b border-white/10">
                            <p className="text-sm font-medium text-slate-200">{user.displayName}</p>
                            <p className="text-xs text-slate-400 truncate">{user.email}</p>

                            {role === "admin" && (
                                <div className="mt-2 inline-flex items-center gap-1 rounded bg-rose-500/10 px-2 py-0.5 text-[10px] font-bold text-rose-400 border border-rose-500/20 uppercase tracking-widest">
                                    <Shield className="h-3 w-3" /> Admin
                                </div>
                            )}
                        </div>

                        <div className="space-y-1">
                            {role === "admin" && (
                                <Link
                                    href="/admin"
                                    onClick={() => setIsOpen(false)}
                                    className="w-full flex items-center gap-2 rounded-lg px-2 py-2 text-sm text-slate-300 hover:bg-white/5 hover:text-white transition-colors"
                                >
                                    <Shield className="h-4 w-4 text-indigo-400" />
                                    Dashboard Quản trị
                                </Link>
                            )}
                            <button
                                onClick={() => {
                                    setIsOpen(false);
                                    logout();
                                }}
                                className="w-full flex justify-start items-center gap-2 rounded-lg px-2 py-2 text-sm text-rose-400 hover:bg-rose-500/10 hover:text-rose-300 transition-colors"
                            >
                                <LogOut className="h-4 w-4" />
                                Đăng xuất
                            </button>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}
