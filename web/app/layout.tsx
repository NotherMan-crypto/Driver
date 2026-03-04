import type { Metadata } from "next";
import { Inter, JetBrains_Mono } from "next/font/google";
import "./globals.css";

const inter = Inter({
  variable: "--font-inter",
  subsets: ["latin", "vietnamese"],
});

const jbMono = JetBrains_Mono({
  variable: "--font-mono",
  subsets: ["latin", "vietnamese"],
});

import { AuthProvider } from "@/components/AuthProvider";
import { UserMenu } from "@/components/UserMenu";

export const metadata: Metadata = {
  title: "Ôn Thi GPLX",
  description: "Hệ thống ôn thi Lý thuyết lái xe",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi">
      <body
        className={`${inter.variable} ${jbMono.variable} font-sans antialiased`}
      >
        <AuthProvider>
          <div className="absolute top-4 right-4 z-50">
            <UserMenu />
          </div>
          {children}
        </AuthProvider>
      </body>
    </html>
  );
}
