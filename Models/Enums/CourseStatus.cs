namespace mvc.Models.Enums
{
    public enum CourseStatus
    {
        /// <summary>
        /// Corso in forma di bozza, visibile solo al docente
        /// </summary>
        Draft,
        /// <summary>
        /// Corso pubblicato, visibile a tutti
        /// </summary>
        Published,
        /// <summary>
        /// Corso eliminato
        /// </summary>
        Deleted
    }
}