public static class UserStatusHelper
    {
        private static readonly Dictionary<UserStatusType, string> UserStatusIdMap = new Dictionary<UserStatusType, string>
        {
            { UserStatusType.Active, "38f2e0ba-ab96-40bb-b9e9-b80a8231e4df" },
            { UserStatusType.Inactive, "a241f717-b088-475f-afc9-39df10f1396b" },
            { UserStatusType.Banned, "414f5cc2-41de-4721-b118-a85629079bee" }
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