using System;

namespace MusicMateAPI.Models
{
    public class UserPreferences
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PreferenceKey { get; set; }
        public string PreferenceValue { get; set; }
    }
}
