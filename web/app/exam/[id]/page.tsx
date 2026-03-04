import { LICENSE_TYPES } from '@/lib/types';
import { getRandomExam } from '@/lib/data';
import QuizView from '../components/QuizView';
import { notFound } from 'next/navigation';

interface Props {
    params: Promise<{ id: string }>;
}

export async function generateMetadata({ params }: Props) {
    const { id } = await params;
    const license = LICENSE_TYPES.find(l => l.id === id);
    if (!license) return { title: 'Không tìm thấy' };

    return {
        title: `Thi thử bằng lái ${license.name} - Online Quiz`,
    };
}

export default async function ExamPage({ params }: Props) {
    const { id } = await params;
    const license = LICENSE_TYPES.find(l => l.id === id);

    if (!license) {
        return notFound();
    }

    // Fetch exam data using server-side logic
    const questions = await getRandomExam(id);

    return <QuizView questions={questions} license={license} />;
}
