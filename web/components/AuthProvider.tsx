"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { User, onAuthStateChanged, signInWithPopup, signOut, signInWithEmailAndPassword, createUserWithEmailAndPassword } from "firebase/auth";
import { auth, googleProvider, db } from "@/lib/firebase.config";
import { doc, getDoc, setDoc } from "firebase/firestore";

interface AuthContextType {
    user: User | null;
    role: "admin" | "student" | null;
    loading: boolean;
    loginWithGoogle: () => Promise<void>;
    loginWithEmail: (e: string, p: string) => Promise<void>;
    registerWithEmail: (e: string, p: string, name: string) => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType>({
    user: null,
    role: null,
    loading: true,
    loginWithGoogle: async () => { },
    loginWithEmail: async () => { },
    registerWithEmail: async () => { },
    logout: async () => { },
});

export const useAuth = () => useContext(AuthContext);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [role, setRole] = useState<"admin" | "student" | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
            setUser(currentUser);

            if (currentUser) {
                // Fetch or Create User Document in Firestore
                try {
                    const userRef = doc(db, "users", currentUser.uid);
                    const userSnap = await getDoc(userRef);

                    if (userSnap.exists()) {
                        setRole(userSnap.data().role as "admin" | "student");
                    } else {
                        // Create target new user config
                        const newUserRole = "student";
                        try {
                            await setDoc(userRef, {
                                uid: currentUser.uid,
                                email: currentUser.email,
                                displayName: currentUser.displayName || currentUser.email?.split('@')[0],
                                photoURL: currentUser.photoURL || "",
                                role: newUserRole,
                                createdAt: new Date()
                            });
                            setRole(newUserRole);
                        } catch (writeErr) {
                            console.error("Lỗi cấp quyền (không thể ghi Role):", writeErr);
                            setRole(newUserRole); // Vẫn cho phép User dùng app dưới quyền Student
                        }
                    }
                } catch (error: any) {
                    console.error("Lỗi đọc user role (Firestore có thể bị chặn Firebase Security Rules hoặc Offline):", error);
                    setRole("student"); // Fallback an toàn cho user
                }
            } else {
                setRole(null);
            }

            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    const loginWithGoogle = async () => {
        try {
            await signInWithPopup(auth, googleProvider);
        } catch (error) {
            console.error("Error logging in with Google", error);
            throw error;
        }
    };

    const loginWithEmail = async (email: string, pass: string) => {
        try {
            await signInWithEmailAndPassword(auth, email, pass);
        } catch (error) {
            console.error("Error logging in with Email", error);
            throw error;
        }
    }

    const registerWithEmail = async (email: string, pass: string, name: string) => {
        try {
            const res = await createUserWithEmailAndPassword(auth, email, pass);
            // Wait for onAuthStateChanged to pick up the user and create the DB record
        } catch (error) {
            console.error("Error registering with Email", error);
            throw error;
        }
    }

    const logout = async () => {
        try {
            await signOut(auth);
        } catch (error) {
            console.error("Error logging out", error);
        }
    };

    return (
        <AuthContext.Provider value={{ user, role, loading, loginWithGoogle, loginWithEmail, registerWithEmail, logout }}>
            {children}
        </AuthContext.Provider>
    );
}
