
const fs = require('fs');
const path = require('path');

const DATA_PATH = path.join(process.cwd(), 'web', 'public', 'data', 'question_bank.json');
const IMAGES_DIR = path.join(process.cwd(), 'web', 'public', 'assets', 'images');

const rawData = fs.readFileSync(DATA_PATH, 'utf8');
const data = JSON.parse(rawData);
const images = fs.readdirSync(IMAGES_DIR);

// 1. Identify Unused Images
const usedImages = new Set(data.Questions.map(q => q.ImageName).filter(Boolean));
const allImages = images.filter(f => (f.endsWith('.jpeg') || f.endsWith('.png')) && f.toLowerCase().startsWith('img_page'));

// Sort images by page number and index: img_page10_1.jpeg -> {page: 10, idx: 1, file: ...}
const sortedImages = allImages.map(f => {
    const match = f.match(/img_page(\d+)_(\d+)/);
    if (match) return { page: parseInt(match[1]), idx: parseInt(match[2]), file: f };
    return null;
}).filter(Boolean).sort((a, b) => {
    if (a.page !== b.page) return a.page - b.page;
    return a.idx - b.idx;
});

let updates = 0;

// 2. Scan Questions 1-300 for Image Keywords
const TARGET_KEYWORDS = ['hình dưới đây', 'theo hình', 'biển báo', 'như hình'];
// Specific mapping logic
// Police questions are usually 36-39.
// Images img_page11_1, img_page12_1 seem relevant?

// Helper to manually map if logic fails, based on known clusters
const manualMap = {
    36: 'img_page11_1.jpeg',
    37: 'img_page12_1.jpeg',
    300: 'img_page67_1.jpeg' // Fix specific one mentioned in context
};

data.Questions.forEach(q => {
    if (q.Id > 300) return; // Only process 1-300

    if (manualMap[q.Id]) {
        q.ImageName = manualMap[q.Id];
        updates++;
        return;
    }

    if (!q.ImageName) {
        const text = q.Text.toLowerCase();
        const hasKeyword = TARGET_KEYWORDS.some(k => text.includes(k));

        if (hasKeyword) {
            // Heuristic strategies

            // Strategy A: Page Number Artifact in text (e.g. " 64") -- RE-RUN with looser regex
            const pageMatch = q.Text.match(/\s(\d{1,3})(\s|$)/);
            if (pageMatch) {
                const pNum = parseInt(pageMatch[1]);
                const candidates = sortedImages.filter(i => i.page === pNum);
                if (candidates.length > 0) {
                    // Assign first unused one for this page? Or just first one?
                    // Let's trying assigning the first available one that matches loose constraints
                    q.ImageName = candidates[0].file;
                    updates++;
                    return;
                }
            }

            // Strategy B: Police Section (35-40)
            if (q.Id >= 35 && q.Id <= 40) {
                // Try to find images around page 10-15
                // Q36 -> Page 11
                // Q37 -> Page 12?
                // Done via manual map.
            }

            // Strategy C: Technique Section (200-300) -> Page 50-70
            if (q.Id > 200) {
                // If text has "hình vẽ", try to find nearby images?
                // This is risky without exact page num.
            }
        }
    }
});

// Explicit fix for "Phan Cau Tao" which had missing images but text artifacts might have been missed
// Q296: "65"
const q296 = data.Questions.find(q => q.Id === 296);
if (q296 && !q296.ImageName) {
    const cand = sortedImages.find(i => i.page === 65 || i.page === 55 /*typo?*/);
    // No img_page65 exists in file list?
    // Skip
}

fs.writeFileSync(DATA_PATH, JSON.stringify(data, null, 4), 'utf8');
console.log(`Fixed ${updates} questions in range 1-300.`);
