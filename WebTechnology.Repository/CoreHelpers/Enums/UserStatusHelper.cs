public static class UserStatusHelper
    {
        private static readonly Dictionary<UserStatusType, string> UserStatusIdMap = new Dictionary<UserStatusType, string>
        {
            { UserStatusType.Active, "38f2e0ba-ab96-40bb-b9e9-b80a8231e4df" },
            { UserStatusType.Inactive, "e4f4c4b4-6c6c-4c4c-9c9c-8c8c8c8c8c8c" }
        };

        public static string ToUserStatusIdString(this UserStatusType userStatusType)
        {
            return UserStatusIdMap[userStatusType];
        }

        public static UserStatusType ToUserStatusType(this string userStatusId)
        {
            return UserStatusIdMap.FirstOrDefault(x => x.Value == userStatusId).Key;
        }
    }