namespace FindMyPrompter.Application.Messages;

public static partial class Messages
{
    public static class Profiles
    {
        public const string UsernameRequired = "Username é obrigatório.";
        public const string UsernameLength = "Username deve ter entre 3 e 30 caracteres.";
        public const string UsernameFormat =
            "Username aceita apenas letras, números, hífen e underscore, começando e terminando com letra ou número.";
        public const string UsernameReserved = "Username reservado pela plataforma.";
        public const string DisplayNameRequired = "Nome de exibição é obrigatório.";
        public const string DisplayNameTooLong = "Nome de exibição deve ter no máximo 80 caracteres.";
        public const string HeadlineTooLong = "Headline deve ter no máximo 160 caracteres.";
        public const string AboutTooLong = "Sobre deve ter no máximo 4000 caracteres.";
        public const string LocationTooLong = "Localização deve ter no máximo 120 caracteres.";
        public const string TooManySkills = "São permitidas no máximo 30 skills.";
        public const string TooManyAiModels = "São permitidos no máximo 30 modelos de IA.";
        public const string TooManyExperiences = "São permitidas no máximo 20 experiências.";
        public const string ExperienceCompanyRequired = "Empresa é obrigatória.";
        public const string ExperienceCompanyTooLong = "Empresa deve ter no máximo 120 caracteres.";
        public const string ExperiencePositionRequired = "Cargo é obrigatório.";
        public const string ExperiencePositionTooLong = "Cargo deve ter no máximo 120 caracteres.";
        public const string ExperienceDescriptionTooLong = "Descrição deve ter no máximo 2000 caracteres.";
        public const string ExperienceLocationTooLong = "Localização deve ter no máximo 120 caracteres.";
        public const string ExperienceStartDateRequired = "Data de início é obrigatória.";
        public const string ExperienceEndBeforeStart =
            "Data de término não pode ser anterior à data de início.";
    }
}
