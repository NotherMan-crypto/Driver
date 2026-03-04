export interface Question {
    Id: number;
    Text: string;
    Answers: string[];
    CorrectIndex: number; // Note: 1-based index from JSON
    IsFatal: boolean;
    Category: string;
    ImageName?: string;
    Explanation?: string;
}

export interface QuestionBank {
    Version: string;
    Questions: Question[];
}

export type QuizMode = 'practice' | 'exam' | 'review';

export interface LicenseType {
    id: string; // 'A1', 'A2', 'B1', etc.
    name: string;
    questionCount: number;
    maxTimeMinutes: number;
    passingScore: number;
    paralysisQuestions: boolean; // Has fatal questions
}

export const LICENSE_TYPES: LicenseType[] = [
    { id: 'A1', name: 'Bằng A1 (Xe máy thông thường)', questionCount: 25, maxTimeMinutes: 19, passingScore: 21, paralysisQuestions: true },
    { id: 'A2', name: 'Bằng A2 (Mô tô PKL >175cc)', questionCount: 25, maxTimeMinutes: 19, passingScore: 23, paralysisQuestions: true },
    { id: 'A3', name: 'Bằng A3 (Mô tô 3 bánh)', questionCount: 25, maxTimeMinutes: 19, passingScore: 23, paralysisQuestions: true },
    { id: 'A4', name: 'Bằng A4 (Máy kéo <1000kg)', questionCount: 25, maxTimeMinutes: 19, passingScore: 23, paralysisQuestions: true },
    { id: 'B1', name: 'Bằng B1 (Ô tô số tự động)', questionCount: 30, maxTimeMinutes: 20, passingScore: 27, paralysisQuestions: true },
    { id: 'B2', name: 'Bằng B2 (Ô tô số sàn)', questionCount: 35, maxTimeMinutes: 22, passingScore: 32, paralysisQuestions: true },
    { id: 'C', name: 'Bằng C (Ô tô tải >3500kg)', questionCount: 40, maxTimeMinutes: 24, passingScore: 36, paralysisQuestions: true },
    { id: 'D', name: 'Bằng D (Ô tô chở 10-30 người)', questionCount: 45, maxTimeMinutes: 26, passingScore: 41, paralysisQuestions: true },
    { id: 'E', name: 'Bằng E (Ô tô >30 người)', questionCount: 45, maxTimeMinutes: 26, passingScore: 41, paralysisQuestions: true },
    { id: 'F', name: 'Bằng F (Các loại xe kéo rơ moóc)', questionCount: 45, maxTimeMinutes: 26, passingScore: 41, paralysisQuestions: true },
];
