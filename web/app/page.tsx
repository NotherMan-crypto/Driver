"use client";

import Link from 'next/link';
import { LICENSE_TYPES } from '@/lib/types';
import { motion, useScroll, useTransform } from 'framer-motion';
import { ArrowRight, ShieldCheck, Zap, Trophy, ChevronRight } from 'lucide-react';
import { useRef } from 'react';
import { useAuth } from '@/components/AuthProvider';

export default function Home() {
  const { user } = useAuth();
  const containerRef = useRef<HTMLDivElement>(null);
  const { scrollYProgress } = useScroll({
    target: containerRef,
    offset: ["start start", "end start"]
  });

  const y1 = useTransform(scrollYProgress, [0, 1], [0, 200]);
  const y2 = useTransform(scrollYProgress, [0, 1], [0, -100]);
  const opacity = useTransform(scrollYProgress, [0, 0.5], [1, 0]);

  return (
    <main ref={containerRef} className="bg-[#050505] text-white overflow-hidden selection:bg-rose-500 selection:text-white">

      {/* 1. HERO SECTION WITH LAMBORGHINI BACKGROUND */}
      <section className="relative min-h-[100svh] w-full flex items-center pt-24 pb-24 lg:pb-32 px-6 md:px-12 lg:px-24">
        {/* Background Image with Parallax & Dark Gradient Mask */}
        <motion.div
          style={{ y: y1, opacity }}
          className="absolute inset-0 z-0"
        >
          {/* Gradients to ensure text readability */}
          <div className="absolute inset-0 bg-gradient-to-t from-[#050505] via-[#050505]/60 to-transparent z-10" />
          <div className="absolute inset-x-0 top-0 h-48 bg-gradient-to-b from-[#050505]/80 to-transparent z-10 pointer-events-none" />
          <div className="absolute inset-y-0 left-0 w-full lg:w-3/4 bg-gradient-to-r from-[#050505]/95 via-[#050505]/70 to-transparent z-10" />

          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src="/assets/images/hero_lambo_bg.png"
            alt="Cinematic Lamborghini scene"
            className="w-full h-full object-cover object-[70%_center]"
          />
        </motion.div>

        {/* Hero Content - Extreme Left Alignment */}
        {/* Note on UserMenu/Auth visibility: We add `pt-16` here to push the hero text down so it doesn't overlap the absolute positioned UserMenu in layout.tsx */}
        <div className="relative z-20 w-full max-w-7xl mx-auto flex flex-col items-start mt-12 md:mt-24 pointer-events-none">

          <motion.div
            initial={{ opacity: 0, y: 50 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8, ease: [0.16, 1, 0.3, 1] }}
            className="flex flex-col space-y-4 mb-8"
          >
            <span className="text-rose-500 font-mono tracking-[0.3em] text-xs md:text-sm uppercase font-bold drop-shadow-lg">
              Phiên bản 2026 • Độc quyền
            </span>
            {/* Adjusted typography size: Smaller than before based on feedback */}
            <h1 className="text-[3rem] md:text-[5rem] lg:text-[6rem] font-black leading-[1.05] tracking-tight drop-shadow-2xl">
              LÀM CHỦ <br />
              <span className="text-transparent bg-clip-text bg-gradient-to-r from-rose-500 to-orange-500 drop-shadow-none">TỐC ĐỘ.</span>
            </h1>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, x: -30 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8, delay: 0.2, ease: "easeOut" }}
            className="w-full md:w-3/4 lg:w-2/5 mb-10"
          >
            <p className="text-slate-300 text-base md:text-lg font-medium leading-relaxed border-l-2 border-rose-500 pl-6 drop-shadow-md">
              Vượt qua kỳ thi sát hạch lý thuyết với nền tảng ôn tập hiện đại nhất.
              Dữ liệu chuẩn xác, phân tích thông minh, thiết kế cho những người khao khát sự hoàn hảo.
            </p>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.6, delay: 0.4 }}
            className="pointer-events-auto"
          >
            {user ? (
              <a href="#licenses" className="group relative overflow-hidden flex items-center gap-4 bg-white text-black px-8 py-4 rounded-lg font-bold text-lg hover:shadow-[0_0_30px_-5px_#f43f5e] transition-all duration-300">
                <span className="relative z-10">CHỌN BẰNG LÁI CỦA BẠN</span>
                <ArrowRight className="w-6 h-6 group-hover:translate-x-2 transition-transform relative z-10" />
              </a>
            ) : (
              <Link href="/auth" className="group relative z-30 flex items-center gap-4 bg-gradient-to-r from-rose-600 to-rose-500 text-white px-8 py-4 rounded-lg font-bold text-lg hover:scale-105 shadow-[0_0_20px_-5px_#f43f5e] transition-all duration-300">
                <span className="relative z-10">THAM GIA NGAY</span>
                <ArrowRight className="w-6 h-6 group-hover:translate-x-2 transition-transform relative z-10" />
              </Link>
            )}
          </motion.div>
        </div>
      </section>

      {/* 2. STATS SECTION (FRAGMENTED & OVERLAPPING) */}
      <section className="relative z-30 -mt-16 md:-mt-24 px-6 md:px-12 lg:px-24 max-w-7xl mx-auto">
        <motion.div
          style={{ y: y2 }}
          className="grid grid-cols-1 md:grid-cols-3 gap-4 md:gap-8"
        >
          <div className="bg-[#0a0a0a]/90 backdrop-blur-md border border-white/10 p-8 flex flex-col justify-between aspect-square hover:border-rose-500/50 transition-colors rounded-2xl shadow-xl">
            <Zap className="w-10 h-10 text-rose-500 mb-8" />
            <div>
              <div className="text-4xl lg:text-5xl font-black mb-2">600+</div>
              <div className="text-slate-400 text-sm font-mono uppercase tracking-widest font-bold">Câu hỏi cập nhật</div>
            </div>
          </div>
          <div className="bg-[#0a0a0a]/90 backdrop-blur-md border border-white/10 p-8 flex flex-col justify-between aspect-square hover:border-orange-500/50 transition-colors md:translate-y-12 rounded-2xl shadow-xl">
            <ShieldCheck className="w-10 h-10 text-orange-500 mb-8" />
            <div>
              <div className="text-4xl lg:text-5xl font-black mb-2">100%</div>
              <div className="text-slate-400 text-sm font-mono uppercase tracking-widest font-bold">Độ tin cậy dữ liệu</div>
            </div>
          </div>
          <div className="bg-gradient-to-br from-rose-600 to-orange-600 p-8 flex flex-col justify-between aspect-square md:translate-y-24 shadow-2xl shadow-rose-900/50 rounded-2xl">
            <Trophy className="w-10 h-10 text-white mb-8" />
            <div>
              <div className="text-4xl lg:text-5xl font-black mb-2">#1</div>
              <div className="text-white/90 text-sm font-mono uppercase tracking-widest font-bold">Nền tảng ôn thi</div>
            </div>
          </div>
        </motion.div>
      </section>

      {/* 3. LICENSE SELECTION AND BOTTOM SECTION */}
      <div className="relative w-full border-t border-white/5 bg-[#050505] overflow-hidden">
        {/* Ambient Glow Backgrounds */}
        <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-rose-600/15 rounded-full blur-[150px] pointer-events-none translate-x-1/3 -translate-y-1/3"></div>
        <div className="absolute bottom-0 left-0 w-[600px] h-[600px] bg-indigo-600/10 rounded-full blur-[150px] pointer-events-none -translate-x-1/3 translate-y-1/3"></div>

        {/* Subtle Grid Pattern */}
        <div className="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.03)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.03)_1px,transparent_1px)] bg-[size:4rem_4rem] [mask-image:radial-gradient(ellipse_80%_50%_at_50%_50%,#000_70%,transparent_100%)] pointer-events-none"></div>

        <section id="licenses" className="relative py-32 md:py-48 px-6 md:px-12 lg:px-24 max-w-7xl mx-auto">

          {/* Typographic Background */}
          <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 text-[15rem] md:text-[25rem] font-black text-white/[0.02] whitespace-nowrap pointer-events-none select-none">
            EXAM
          </div>

          <div className="relative z-10 w-full mb-16 md:mb-24 flex flex-col md:flex-row md:items-end justify-between gap-8 border-b border-white/10 pb-8">
            <div>
              <h2 className="text-4xl md:text-5xl font-black">CHỌN HẠNG BẰNG.</h2>
              <p className="text-slate-400 mt-4 max-w-md font-medium">Bắt đầu bài thi sát hạch tiêu chuẩn được cá nhân hóa theo từng hạng giấy phép.</p>
            </div>

            {!user && (
              <div className="bg-rose-500/10 border border-rose-500/30 p-4 rounded-lg backdrop-blur-sm">
                <p className="text-sm text-rose-400 font-mono flex items-center gap-2 font-bold">
                  <span className="w-2 h-2 rounded-full bg-rose-500 animate-pulse"></span>
                  YÊU CẦU ĐĂNG NHẬP ĐỂ SỬ DỤNG
                </p>
              </div>
            )}
          </div>

          {user ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 relative z-10">
              {LICENSE_TYPES.map((license, i) => (
                <motion.div
                  key={license.id}
                  initial={{ opacity: 0, y: 30 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true, margin: "-100px" }}
                  transition={{ duration: 0.5, delay: i * 0.1 }}
                >
                  <Link
                    href={`/exam/${license.id}`}
                    className="group block bg-white/[0.03] backdrop-blur-xl border border-white/10 p-8 hover:bg-white hover:text-black hover:border-white shadow-[0_8px_30px_rgb(0,0,0,0.12)] hover:shadow-[0_8px_30px_rgba(244,63,94,0.2)] transition-all duration-300 relative overflow-hidden rounded-2xl"
                  >
                    <div className="absolute top-0 right-0 p-4 opacity-0 group-hover:opacity-100 transform translate-x-4 group-hover:translate-x-0 transition-all duration-300">
                      <ChevronRight className="w-8 h-8" />
                    </div>

                    <div className="text-5xl font-black mb-6 text-slate-600 group-hover:text-black transition-colors">{license.id}</div>
                    <h3 className="text-xl font-black mb-2">{license.name.split('(')[0].trim()}</h3>

                    <div className="w-full h-[2px] bg-white/10 group-hover:bg-black/10 my-6 transition-colors"></div>

                    <div className="flex justify-between items-center text-sm font-mono text-slate-400 group-hover:text-slate-600 transition-colors font-bold">
                      <span>{license.questionCount} CÂU HỎI</span>
                      <span>{license.maxTimeMinutes} PHÚT</span>
                    </div>
                  </Link>
                </motion.div>
              ))}
            </div>
          ) : (
            <div className="relative z-10 bg-white/[0.02] backdrop-blur-xl border border-white/10 p-12 text-center flex flex-col items-center rounded-3xl shadow-2xl">
              <div className="bg-white/5 p-4 rounded-full mb-6 border border-white/5">
                <ShieldCheck className="w-16 h-16 text-slate-300" />
              </div>
              <h3 className="text-3xl font-black mb-4">Nội Dung Độc Quyền</h3>
              <p className="text-slate-400 max-w-lg mb-8 font-medium">Bạn cần là thành viên để truy cập ngân hàng 600 câu hỏi và hệ thống thi thử mô phỏng thực tế.</p>
              <Link href="/auth" className="bg-white text-black px-8 py-4 rounded-xl font-bold text-lg hover:bg-slate-200 shadow-xl hover:shadow-2xl transition-all">
                ĐĂNG NHẬP HOẶC ĐĂNG KÝ
              </Link>
            </div>
          )}
        </section>

        {/* FOOTER */}
        <footer className="border-t border-white/10 py-12 px-6 lg:px-24 bg-transparent relative z-20">
          <div className="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center gap-4 text-xs font-mono text-slate-500 font-bold uppercase">
            <div>© 2026 NOTHERMAN DRIVER</div>
            <div className="flex gap-6">
              <span className="hover:text-white cursor-pointer transition-colors">Privacy</span>
              <span className="hover:text-white cursor-pointer transition-colors">Terms</span>
            </div>
          </div>
        </footer>
      </div>
    </main>
  );
}
