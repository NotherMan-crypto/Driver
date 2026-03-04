const fs = require('fs');

const schema = JSON.parse(fs.readFileSync('./question_schema.json', 'utf8'));
const sample = JSON.parse(fs.readFileSync('./data/sample_question.json', 'utf8'));

console.log('=== Schema Validation Report ===\n');

// Check required fields
console.log('1. Required Fields Check:');
const required = schema.required;
let allPresent = true;
required.forEach(f => {
    const present = sample.hasOwnProperty(f);
    console.log(`   ${present ? '✓' : '✗'} ${f}: ${present ? 'OK' : 'MISSING'}`);
    if (!present) allPresent = false;
});

// Check enums
console.log('\n2. Enum Validation:');
const categories = schema.$defs.Category.enum;
const categoryValid = categories.includes(sample.category);
console.log(`   ${categoryValid ? '✓' : '✗'} category: "${sample.category}" ${categoryValid ? 'is valid' : 'INVALID'}`);

const licenseTypes = schema.$defs.LicenseType.enum;
const allLicensesValid = sample.license_types.every(lt => licenseTypes.includes(lt));
console.log(`   ${allLicensesValid ? '✓' : '✗'} license_types: ${allLicensesValid ? 'all valid' : 'INVALID'}`);

// Check answers structure
console.log('\n3. Answers Structure:');
const answersValid = sample.answers.length >= 2 && sample.answers.length <= 4;
console.log(`   ${answersValid ? '✓' : '✗'} answers count: ${sample.answers.length} (valid: 2-4)`);

const correctIdValid = sample.answers.some(a => a.id === sample.correct_answer_id);
console.log(`   ${correctIdValid ? '✓' : '✗'} correct_answer_id: ${sample.correct_answer_id} ${correctIdValid ? 'exists in answers' : 'NOT FOUND'}`);

// Summary
console.log('\n=== RESULT ===');
if (allPresent && categoryValid && allLicensesValid && answersValid && correctIdValid) {
    console.log('✓ Sample question PASSES schema validation!');
    process.exit(0);
} else {
    console.log('✗ Sample question FAILS validation');
    process.exit(1);
}
