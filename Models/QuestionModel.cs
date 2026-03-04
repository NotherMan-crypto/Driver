using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace TracNghiemLaiXe.Models
{
    /// <summary>
    /// Official Vietnamese driving exam categories
    /// </summary>
    public enum QuestionCategory
    {
        [JsonPropertyName("concepts_and_rules")]
        ConceptsAndRules,

        [JsonPropertyName("transport_business")]
        TransportBusiness,

        [JsonPropertyName("culture_and_ethics")]
        CultureAndEthics,

        [JsonPropertyName("driving_techniques")]
        DrivingTechniques,

        [JsonPropertyName("construction_repair")]
        ConstructionRepair,

        [JsonPropertyName("road_signs")]
        RoadSigns,

        [JsonPropertyName("situational_analysis")]
        SituationalAnalysis
    }

    /// <summary>
    /// Vietnamese driving license types
    /// </summary>
    public enum LicenseType
    {
        A1,
        A2,
        A3,
        A4,
        B1,
        B2,
        C,
        D,
        E,
        F
    }

    /// <summary>
    /// Represents a single answer option
    /// </summary>
    public class Answer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a driving theory quiz question matching CSGT-2025 JSON format
    /// </summary>
    public class Question
    {


        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; } = string.Empty;

        [JsonPropertyName("answers")]
        public List<Answer> AnswerTexts { get; set; } = new();

        [JsonPropertyName("correct_answer_id")]
        public int CorrectIndex { get; set; }

        [JsonPropertyName("is_paralysis")]
        public bool IsParalysis { get; set; }

        [JsonPropertyName("category")]
        public string CategoryRaw { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public string? ImagePath { get; set; }

        // Computed properties for backward compatibility
        [JsonIgnore]
        public int CorrectAnswerId
        {
            get => CorrectIndex;
            set => CorrectIndex = value;
        }

        [JsonIgnore]
        public List<Answer> Answers => AnswerTexts;

        [JsonIgnore]
        public QuestionCategory Category
        {
            get
            {
                return CategoryRaw switch
                {
                    "Quy tắc giao thông" => QuestionCategory.ConceptsAndRules,
                    "Nghiệp vụ vận tải" => QuestionCategory.TransportBusiness,
                    "Văn hóa, đạo đức" => QuestionCategory.CultureAndEthics,
                    "Kỹ thuật lái xe" => QuestionCategory.DrivingTechniques,
                    "Cấu tạo, sửa chữa" => QuestionCategory.ConstructionRepair,
                    "Biển báo" => QuestionCategory.RoadSigns,
                    "Giải thế sa hình" => QuestionCategory.SituationalAnalysis,
                    _ => QuestionCategory.ConceptsAndRules
                };
            }
        }

        /// <summary>
        /// Computed property for cleaner XAML visibility binding.
        /// </summary>
        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);
    }

    /// <summary>
    /// Represents the question bank container matching CSGT-2025 JSON format
    /// </summary>
    public class QuestionBank
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("questions")]
        public List<Question> Questions { get; set; } = new();
    }

    /// <summary>
    /// JSON converter for QuestionCategory enum with snake_case support
    /// </summary>
    public class QuestionCategoryConverter : JsonConverter<QuestionCategory>
    {
        public override QuestionCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value switch
            {
                "concepts_and_rules" => QuestionCategory.ConceptsAndRules,
                "transport_business" => QuestionCategory.TransportBusiness,
                "culture_and_ethics" => QuestionCategory.CultureAndEthics,
                "driving_techniques" => QuestionCategory.DrivingTechniques,
                "construction_repair" => QuestionCategory.ConstructionRepair,
                "road_signs" => QuestionCategory.RoadSigns,
                "situational_analysis" => QuestionCategory.SituationalAnalysis,
                _ => throw new JsonException($"Unknown category: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, QuestionCategory value, JsonSerializerOptions options)
        {
            var stringValue = value switch
            {
                QuestionCategory.ConceptsAndRules => "concepts_and_rules",
                QuestionCategory.TransportBusiness => "transport_business",
                QuestionCategory.CultureAndEthics => "culture_and_ethics",
                QuestionCategory.DrivingTechniques => "driving_techniques",
                QuestionCategory.ConstructionRepair => "construction_repair",
                QuestionCategory.RoadSigns => "road_signs",
                QuestionCategory.SituationalAnalysis => "situational_analysis",
                _ => throw new JsonException($"Unknown category: {value}")
            };
            writer.WriteStringValue(stringValue);
        }
    }

    /// <summary>
    /// JSON converter for List of enums with string support
    /// </summary>
    public class JsonStringEnumListConverter<T> : JsonConverter<List<T>> where T : struct, Enum
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<T>();
            
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    if (Enum.TryParse<T>(stringValue, ignoreCase: true, out var enumValue))
                    {
                        list.Add(enumValue);
                    }
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item.ToString());
            }
            writer.WriteEndArray();
        }
    }
}
