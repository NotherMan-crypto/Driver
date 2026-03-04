# Question Schema Design Plan

> **Project Type:** BACKEND (Schema/Data Definition)  
> **Agent:** `backend-specialist`  
> **Phase:** PLANNING → SOLUTIONING

---

## Overview

Define a comprehensive JSON schema for Vietnamese driving theory quiz questions. The schema must support:
- **Hybrid data management** (bundled JSON + remote API sync)
- **7 official exam categories** (flat structure)
- **10 license types** with many-to-many relationships
- **Paralysis questions** (immediate failure on incorrect answer)
- **Versioning** for law updates without app redeployment

---

## Success Criteria

| Criteria | Metric |
|----------|--------|
| Schema validates all field types | JSON Schema validation passes |
| Supports multi-license questions | Array of license types per question |
| Sync-compatible | Includes `version` + `last_updated` fields |
| Mobile-ready | Optimized for offline-first MAUI app |

---

## Tech Stack

| Component | Technology | Rationale |
|-----------|------------|-----------|
| Schema Format | JSON Schema (Draft 2020-12) | Industry standard, validation support |
| Storage | Bundled JSON file | Offline-first performance |
| Sync | REST API (future) | Centralized updates |

---

## Schema Design

### Core Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | Integer | ✅ | Unique question identifier |
| `question_text` | String | ✅ | The question content (Vietnamese) |
| `image_url` | String \| null | ❌ | Local asset path (e.g., `assets/images/q001.png`) |
| `answers` | Array\<Answer\> | ✅ | 2-4 answer options |
| `correct_answer_id` | Integer | ✅ | ID of the correct answer |
| `category` | Enum | ✅ | One of 7 official categories |
| `license_types` | Array\<Enum\> | ✅ | Applicable license types (Many-to-Many) |
| `is_paralysis` | Boolean | ✅ | If true, wrong answer = immediate failure |
| `explanation` | String | ✅ | Explanation for review mode |
| `version` | Integer | ✅ | Schema/content version for sync |
| `last_updated` | DateTime (ISO 8601) | ✅ | Last modification timestamp |

### Answer Object

| Field | Type | Description |
|-------|------|-------------|
| `id` | Integer | Answer identifier (1-4) |
| `text` | String | Answer content (Vietnamese) |

### Enums

**Category Enum (7 values):**
```
concepts_and_rules      # Khái niệm và Quy tắc
transport_business      # Nghiệp vụ Vận tải
culture_and_ethics      # Văn hóa và Đạo đức
driving_techniques      # Kỹ thuật Lái xe
construction_repair     # Cấu tạo và Sửa chữa
road_signs              # Biển báo Đường bộ
situational_analysis    # Sa Hình
```

**License Type Enum (10 values):**
```
A1, A2, A3, A4, B1, B2, C, D, E, F
```

---

## File Structure

```
d:\TracNghiemLaiXe\
├── question_schema.json      # JSON Schema definition
├── data/
│   └── questions.json        # Actual question bank (future)
└── assets/
    └── images/               # Question images (future)
```

---

## Task Breakdown

### Task 1: Create JSON Schema File
| Attribute | Value |
|-----------|-------|
| **Agent** | `backend-specialist` |
| **Skill** | `api-patterns`, `database-design` |
| **Priority** | P0 |
| **Dependencies** | None |
| **INPUT** | This plan document |
| **OUTPUT** | `question_schema.json` with full validation rules |
| **VERIFY** | Schema validates sample question JSON |

---

## Example Question (Conforming to Schema)

```json
{
  "id": 1,
  "question_text": "Khái niệm 'phương tiện giao thông cơ giới đường bộ' được hiểu như thế nào?",
  "image_url": null,
  "answers": [
    { "id": 1, "text": "Gồm xe ô tô, máy kéo, xe mô tô hai bánh, xe gắn máy và các loại xe tương tự." },
    { "id": 2, "text": "Gồm xe ô tô, máy kéo, rơ moóc hoặc sơ mi rơ moóc được kéo bởi xe ô tô, máy kéo." },
    { "id": 3, "text": "Gồm xe ô tô, máy kéo, rơ moóc hoặc sơ mi rơ moóc được kéo bởi xe ô tô, máy kéo, xe mô tô hai bánh, xe mô tô ba bánh, xe gắn máy và các loại xe tương tự." }
  ],
  "correct_answer_id": 3,
  "category": "concepts_and_rules",
  "license_types": ["A1", "A2", "B1", "B2"],
  "is_paralysis": false,
  "explanation": "Theo Luật Giao thông đường bộ, phương tiện giao thông cơ giới đường bộ bao gồm tất cả các loại xe có động cơ.",
  "version": 1,
  "last_updated": "2026-01-28T11:45:00+07:00"
}
```

---

## Phase X: Verification Checklist

- [ ] JSON Schema syntax is valid
- [ ] All 7 categories defined as enum
- [ ] All 10 license types defined as enum
- [ ] `is_paralysis` boolean field present
- [ ] `version` + `last_updated` fields for sync
- [ ] Sample question validates against schema
- [ ] Schema is compatible with MAUI offline storage

---

## Next Steps

After plan approval:
1. Run `/create question_schema.json` to implement the schema
2. Create sample question bank with 5-10 questions for validation
3. Document sync API contract (future task)
