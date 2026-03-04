
const fs = require('fs');
const path = require('path');

const DATA_PATH = path.join(process.cwd(), 'web', 'public', 'data', 'question_bank.json');
const IMAGES_DIR = path.join(process.cwd(), 'web', 'public', 'assets', 'images');

// Read data
const rawData = fs.readFileSync(DATA_PATH, 'utf8');
const data = JSON.parse(rawData);
const images = fs.readdirSync(IMAGES_DIR);

let updatedCount = 0;
let extensionFixedCount = 0;
let linkedCount = 0;

// Helper to find image file case-insensitive/extension-agnostic
function findImageFile(name) {
    if (!name) return null;
    const baseName = path.parse(name).name.toLowerCase(); // q301
    // Try to find matching file in images array
    const match = images.find(f => path.parse(f).name.toLowerCase() === baseName);
    return match || null;
}

// Regex to find trailing page number " 64", " 65" etc.
const PAGE_NUM_REGEX = /\s(\d+)\s*$/;

data.Questions.forEach((q, index) => {
    let originalImage = q.ImageName;
    let newImage = originalImage;

    // 1. Fix Extension (.png -> .jpeg)
    if (originalImage) {
        const found = findImageFile(originalImage);
        if (found && found !== originalImage) {
            newImage = found;
            extensionFixedCount++;
        }
    }

    // 2. Try to find missing image using Page Number artifact
    if (!newImage) {
        // Search for page number artifact in Text or Answers
        let pageNum = null;

        // Check Text
        const textMatch = q.Text.match(PAGE_NUM_REGEX);
        if (textMatch) pageNum = textMatch[1];

        // Check Answers if not found in Text
        if (!pageNum && q.Answers) {
            for (const ans of q.Answers) {
                const ansMatch = ans.match(PAGE_NUM_REGEX);
                if (ansMatch) {
                    pageNum = ansMatch[1];
                    break;
                }
            }
        }

        if (pageNum) {
            // Look for img_pageX_...
            const candidates = images.filter(img => img.startsWith(`img_page${pageNum}_`));
            if (candidates.length > 0) {
                // Sort to pick likely first one (or maybe checking question index on page?)
                // Simple heuristic: pick the first one.
                // Ideally we'd map "1st question on page" -> "_1", but we don't know page boundaries easily.
                // Let's just assign the first one found for now, better than nothing.
                // Improve: Try to use index to distribute? Sticky...
                // Safe bet: Assign the first candidate.
                newImage = candidates[0];
                linkedCount++;
            }
        }
    }

    if (newImage !== q.ImageName) {
        q.ImageName = newImage;
        updatedCount++;
    }
});

// Write back
fs.writeFileSync(DATA_PATH, JSON.stringify(data, null, 4), 'utf8');

console.log(`Update Complete:
- Total Updated: ${updatedCount}
- Extensions Fixed: ${extensionFixedCount}
- Missing Linked: ${linkedCount}
`);
