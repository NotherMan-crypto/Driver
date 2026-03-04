'use client';

import { useState, useEffect } from 'react';
import { Question, LicenseType } from '@/lib/types';
import Link from 'next/link';

interface QuizViewProps {
    questions: Question[];
    license: LicenseType;
}

function QuizView({ questions, license }: QuizViewProps) {
    // ... The rest of the component body ...
    const [currentIndex, setCurrentIndex] = useState(0);
    const [answers, setAnswers] = useState<Record<number, number>>({}); // QuestionID -> AnswerIndex (0-based)
    const [timeLeft, setTimeLeft] = useState(license.maxTimeMinutes * 60);
    const [isSubmitted, setIsSubmitted] = useState(false);

    if (!questions || questions.length === 0) {
        return (
            <div className="min-h-screen flex items-center justify-center text-white">
                <div className="text-center">
                    <h2 className="text-2xl font-bold mb-4">Không có dữ liệu câu hỏi</h2>
                    <p className="text-slate-400 mb-6">Vui lòng thử lại sau hoặc liên hệ quản trị viên.</p>
                    <Link href="/" className="px-6 py-2 bg-indigo-600 rounded-lg hover:bg-indigo-500 transition">
                        Về trang chủ
                    </Link>
                </div>
            </div>
        );
    }

    // Timer logic
    useEffect(() => {
        if (isSubmitted || timeLeft <= 0) return;
        const timer = setInterval(() => {
            setTimeLeft((prev) => {
                if (prev <= 1) {
                    setIsSubmitted(true);
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);
        return () => clearInterval(timer);
    }, [isSubmitted, timeLeft]);

    // Keyboard Navigation
    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'ArrowRight') {
                setCurrentIndex(prev => Math.min(prev + 1, questions.length - 1));
            } else if (e.key === 'ArrowLeft') {
                setCurrentIndex(prev => Math.max(prev - 1, 0));
            }
        };

        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [questions.length]);

    // Format time mm:ss
    const formatTime = (seconds: number) => {
        const m = Math.floor(seconds / 60);
        const s = seconds % 60;
        return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
    };

    const currentQuestion = questions[currentIndex];
    // Calculate selected index for UI (if answered)
    const selectedAnswerIdx = answers[currentQuestion.Id];

    const handleSelectAnswer = (idx: number) => {
        if (isSubmitted) return;
        setAnswers(prev => ({
            ...prev,
            [currentQuestion.Id]: idx
        }));
    };

    const handleSubmit = () => {
        if (confirm('Bạn có chắc chắn muốn nộp bài?')) {
            setIsSubmitted(true);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    };

    // Helper to get answer style based on state
    const getAnswerStyle = (idx: number) => {
        if (!isSubmitted) {
            if (selectedAnswerIdx === idx) return 'bg-indigo-600 border-indigo-500 text-white';
            return 'bg-slate-800/50 hover:bg-slate-700/50 border-slate-700 text-slate-300';
        }

        // Result Mode
        const isCorrect = idx + 1 === currentQuestion.CorrectIndex; // JSON uses 1-based index
        const isSelected = selectedAnswerIdx === idx;

        if (isCorrect) return 'bg-green-600 border-green-500 text-white'; // Correct answer
        if (isSelected && !isCorrect) return 'bg-red-600 border-red-500 text-white'; // Wrong choice
        if (isSelected && isCorrect) return 'bg-green-600 border-green-500 text-white'; // Correct choice

        return 'bg-slate-800/50 opacity-50 border-slate-700 text-slate-400';
    };

    return (
        <div className="min-h-screen flex flex-col lg:flex-row max-w-7xl mx-auto p-4 gap-6">

            {/* Left: Question Area */}
            <div className="flex-1 flex flex-col space-y-6">
                {/* Header Mobile */}
                <div className="flex justify-between items-center lg:hidden glass p-4 rounded-xl">
                    <Link href="/" className="text-slate-400 hover:text-white">← Thoát</Link>
                    <div className={`font-mono text-xl font-bold ${timeLeft < 60 ? 'text-red-500' : 'text-indigo-400'}`}>
                        {formatTime(timeLeft)}
                    </div>
                </div>

                {/* Result Summary */}
                {isSubmitted && (() => {
                    let correctCount = 0;
                    let hasFailedFatal = false;
                    questions.forEach((q) => {
                        const isCorrect = answers[q.Id] !== undefined && answers[q.Id] + 1 === q.CorrectIndex;
                        if (isCorrect) correctCount++;
                        else if (q.IsFatal && (answers[q.Id] !== undefined || true)) {
                            if (answers[q.Id] !== undefined && answers[q.Id] + 1 !== q.CorrectIndex) {
                                hasFailedFatal = true;
                            } else if (answers[q.Id] === undefined) {
                                hasFailedFatal = true;
                            }
                        }
                    });
                    const isPassed = correctCount >= license.passingScore && !hasFailedFatal;
                    return (
                        <div className={`p-6 rounded-2xl border-2 shadow-lg mb-6 text-center ${isPassed ? 'bg-green-600/20 border-green-500 shadow-green-500/20' : 'bg-red-600/20 border-red-500 shadow-red-500/20'}`}>
                            <h2 className={`text-2xl font-bold mb-2 ${isPassed ? 'text-green-400' : 'text-red-400'}`}>
                                {isPassed ? 'CHÚC MỪNG! BẠN ĐÃ ĐẠT' : 'RẤT TIẾC! BẠN KHÔNG ĐẠT'}
                            </h2>
                            <p className="text-lg text-slate-200">
                                Số điểm: <span className="font-bold">{correctCount}/{questions.length}</span> (Yêu cầu: {license.passingScore})
                            </p>
                            {hasFailedFatal && <p className="text-red-400 mt-2 font-semibold">Lý do: Sai hoặc bỏ trống câu điểm liệt.</p>}
                        </div>
                    );
                })()}

                {/* Question Card */}
                <div className="glass p-6 md:p-8 rounded-2xl flex-1 flex flex-col relative overflow-hidden">
                    {/* Decor */}
                    <div className="absolute top-0 left-0 w-1 h-full bg-indigo-500/50"></div>

                    <div className="mb-6">
                        <span className="text-indigo-400 text-sm font-bold tracking-wider uppercase mb-2 block">
                            Câu hỏi {currentIndex + 1} / {questions.length}
                        </span>
                        <h2 className="text-xl md:text-2xl font-semibold leading-relaxed text-slate-100">
                            {currentQuestion.Text}
                        </h2>
                        {currentQuestion.IsFatal && (
                            <span className="inline-block mt-2 px-2 py-1 bg-red-500/10 text-red-400 text-xs rounded border border-red-500/20">
                                ⚠️ Câu điểm liệt
                            </span>
                        )}
                    </div>

                    {/* Image Place holder */}
                    {/* Image Display */}
                    {currentQuestion.ImageName && (
                        <div className="mb-6 rounded-lg overflow-hidden border border-slate-700 bg-slate-900/50 flex items-center justify-center">
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img
                                src={`/assets/images/${currentQuestion.ImageName}`}
                                alt="Hình minh họa câu hỏi"
                                className="max-h-64 md:max-h-80 object-contain w-auto h-auto"
                                onError={(e) => {
                                    const target = e.target as HTMLImageElement;
                                    if (!target.dataset.triedFallback) {
                                        target.dataset.triedFallback = 'true';
                                        if (target.src.endsWith('.jpeg')) {
                                            target.src = target.src.replace('.jpeg', '.png');
                                        } else if (target.src.endsWith('.png')) {
                                            target.src = target.src.replace('.png', '.jpeg');
                                        } else if (target.src.endsWith('.jpg')) {
                                            target.src = target.src.replace('.jpg', '.png');
                                        }
                                    } else {
                                        const parent = target.parentElement;
                                        if (parent) parent.style.display = 'none';
                                    }
                                }}
                            />
                        </div>
                    )}

                    {/* Answers */}
                    <div className="space-y-3 flex-1">
                        {currentQuestion.Answers.map((ans, idx) => (
                            <button
                                key={idx}
                                onClick={() => handleSelectAnswer(idx)}
                                className={`w-full text-left p-4 rounded-xl border transition-all duration-200 flex items-start group ${getAnswerStyle(idx)}`}
                            >
                                <div className={`w-6 h-6 rounded-full border-2 mr-4 flex-shrink-0 flex items-center justify-center mt-0.5
                                  ${selectedAnswerIdx === idx ? 'border-white bg-white/20' : 'border-slate-500 group-hover:border-indigo-400'}`}>
                                    {selectedAnswerIdx === idx && <div className="w-2.5 h-2.5 bg-white rounded-full"></div>}
                                </div>
                                <span className="text-base md:text-lg">{ans}</span>
                            </button>
                        ))}
                    </div>

                    {/* Explanation (Result Mode) */}
                    {isSubmitted && (
                        <div className="mt-6 p-4 rounded-xl bg-indigo-500/10 border border-indigo-500/20 text-indigo-200 text-sm">
                            <strong>Giải thích:</strong> {currentQuestion.Explanation || "Không có giải thích chi tiết."}
                        </div>
                    )}
                </div>

                {/* Navigation Bar */}
                <div className="glass p-4 rounded-xl flex flex-col sm:flex-row justify-between items-center gap-4">
                    <div className="flex justify-between w-full sm:w-auto gap-4">
                        <button
                            disabled={currentIndex === 0}
                            onClick={() => setCurrentIndex(p => p - 1)}
                            className="flex-1 sm:flex-none px-6 py-2 rounded-lg bg-slate-800 hover:bg-slate-700 disabled:opacity-50 text-white transition"
                        >
                            Trước
                        </button>
                        <button
                            disabled={currentIndex === questions.length - 1}
                            onClick={() => setCurrentIndex(p => p + 1)}
                            className="flex-1 sm:flex-none px-6 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white font-medium transition"
                        >
                            Sau
                        </button>
                    </div>
                    <span className="text-slate-400 hidden sm:block text-sm">Dùng phím mũi tên để chuyển câu</span>
                    <div className="w-full sm:hidden">
                        <button
                            onClick={handleSubmit}
                            disabled={isSubmitted}
                            className="w-full py-3 rounded-xl bg-gradient-to-r from-red-500 to-pink-600 text-white font-bold disabled:opacity-50 transition-all"
                        >
                            {isSubmitted ? 'Đã nộp bài' : 'Nộp bài thi'}
                        </button>
                    </div>
                </div>
            </div>

            {/* Right: Sidebar (Grid & Info) */}
            <div className="hidden lg:flex w-80 flex-col space-y-6">

                {/* Timer Card */}
                <div className="glass p-6 rounded-2xl text-center">
                    <div className="text-sm text-slate-400 mb-1">Thời gian còn lại</div>
                    <div className={`text-4xl font-mono font-bold ${timeLeft < 60 ? 'text-red-500 animate-pulse' : 'text-white'}`}>
                        {formatTime(timeLeft)}
                    </div>
                </div>

                {/* Question Map */}
                <div className="glass p-6 rounded-2xl flex-1 flex flex-col">
                    <h3 className="text-lg font-bold text-white mb-4">Danh sách câu hỏi</h3>
                    <div className="grid grid-cols-5 gap-2 content-start flex-1 overflow-y-auto pr-1">
                        {questions.map((q, idx) => {
                            let statusClass = 'bg-slate-800 text-slate-400 hover:bg-slate-700'; // Default

                            const answered = answers[q.Id] !== undefined;
                            if (answered) statusClass = 'bg-indigo-500/40 text-white border border-indigo-500/60';
                            if (currentIndex === idx) statusClass = 'bg-indigo-600 text-white border-2 border-indigo-400 shadow-lg shadow-indigo-500/30';

                            if (isSubmitted) {
                                const isCorrect = (answers[q.Id] !== undefined) && (answers[q.Id] + 1 === q.CorrectIndex);
                                statusClass = isCorrect ? 'bg-green-500/80 text-white' : 'bg-red-500/80 text-white';
                                if (currentIndex === idx) statusClass += ' ring-2 ring-white';
                            }

                            return (
                                <button
                                    key={q.Id}
                                    onClick={() => setCurrentIndex(idx)}
                                    className={`aspect-square rounded-lg flex items-center justify-center text-sm font-medium transition-all ${statusClass}`}
                                >
                                    {idx + 1}
                                </button>
                            );
                        })}
                    </div>

                    <button
                        onClick={handleSubmit}
                        disabled={isSubmitted}
                        className="w-full mt-6 py-3 rounded-xl bg-gradient-to-r from-red-500 to-pink-600 hover:from-red-600 hover:to-pink-700 text-white font-bold shadow-lg shadow-red-500/20 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                    >
                        {isSubmitted ? 'Đã nộp bài' : 'Nộp bài thi'}
                    </button>
                </div>
            </div>
        </div>
    );
}
import { withAuth } from '@/components/WithAuth';
export default withAuth(QuizView);
