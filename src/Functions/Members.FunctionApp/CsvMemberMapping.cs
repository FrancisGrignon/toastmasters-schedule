using TinyCsvParser.Mapping;

namespace Members.FunctionApp
{
    public class CsvMemberMapping : CsvMapping<CsvMemberRow>
    {
        //  0. Customer ID
        //  1. Name
        //  2. Company / In Care Of
        //  3. Addr L1
        //  4. Addr L2
        //  5. Addr L3
        //  6. Addr L4
        //  7. Addr L5
        //  8. Country
        //  9. Member has opted-out of Toastmasters WHQ marketing mail
        // 10. Email
        // 11. Secondary Email
        // 12. Member has opted-out of Toastmasters WHQ marketing emails
        // 13. Home Phone
        // 14. Mobile Phone
        // 15. Additional Phone
        // 16. Member has opted-out of Toastmasters WHQ marketing phone calls
        // 17. Paid Until
        // 18. Member of Club Since
        // 19. Original Join Date
        // 20. status (*)
        // 21. Current Position
        // 22. Future Position
        // 23. Pathways Enrolled
        public CsvMemberMapping() : base()
        {
            MapProperty(0, x => x.ToastmastersId);
            MapProperty(1, x => x.Name);
            MapProperty(10, x => x.Email);
        }
    }
}
