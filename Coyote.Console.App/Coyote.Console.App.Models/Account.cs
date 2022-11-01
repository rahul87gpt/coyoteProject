using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coyote.Console.App.Models
{
    [Table("Account")]
    public class Accounts
    {
#pragma warning disable CA1707 // Identifiers should not contain underscores
        [Key]
        public int ACC_NUMBER { get; set; }
        public string ACC_CARD_NUMBER { get; set; }
        public string ACC_ADD_ID_1 { get; set; }
        public string ACC_ADD_ID_2 { get; set; }
        public string ACC_ADD_ID_3 { get; set; }
        public string ACC_ADD_ID_4 { get; set; }
        public string ACC_ADD_ID_5 { get; set; }
        public string ACC_ADD_ID_6 { get; set; }
        public string ACC_ADD_ID_7 { get; set; }
        public string ACC_ADD_ID_8 { get; set; }
        public string ACC_ADD_ID_9 { get; set; }
        public string ACC_ADD_ID_10 { get; set; }
        public string ACC_ADD_ID_11 { get; set; }
        public string ACC_ADD_ID_12 { get; set; }
        public int? ACC_ADD_ID_TYPE_1 { get; set; }
        public int? ACC_ADD_ID_TYPE_2 { get; set; }
        public int? ACC_ADD_ID_TYPE_3 { get; set; }
        public int? ACC_ADD_ID_TYPE_4 { get; set; }
        public int? ACC_ADD_ID_TYPE_5 { get; set; }
        public int? ACC_ADD_ID_TYPE_6 { get; set; }
        public int? ACC_ADD_ID_TYPE_7 { get; set; }
        public int? ACC_ADD_ID_TYPE_8 { get; set; }
        public int? ACC_ADD_ID_TYPE_9 { get; set; }
        public int? ACC_ADD_ID_TYPE_10 { get; set; }
        public int? ACC_ADD_ID_TYPE_11 { get; set; }
        public int? ACC_ADD_ID_TYPE_12 { get; set; }
        public int? ACC_MASTER_ACC { get; set; }
        public string ACC_TITLE { get; set; }
        public string ACC_FIRST_NAME { get; set; }
        public string ACC_SURNAME { get; set; }
        public string ACC_ADDR_1 { get; set; }
        public string ACC_ADDR_2 { get; set; }
        public string ACC_ADDR_3 { get; set; }
        public string ACC_POST_CODE { get; set; }
        public string ACC_ACCOUNT_NAME { get; set; }
        public string ACC_CONTACT { get; set; }
        public string ACC_PHONE { get; set; }
        public string ACC_MOBILE { get; set; }
        public string ACC_FAX { get; set; }
        public string ACC_EMAIL { get; set; }
        public int? ACC_GENDER { get; set; }
        public int? ACC_STATUS { get; set; }
        public string ACC_DATE_OF_BIRTH { get; set; }
        public string ACC_OCCUPATION { get; set; }
        public string ACC_DATE_ADDED { get; set; }
        public string ACC_DATE_CHANGE { get; set; }
        public string ACC_TILL_IND { get; set; }
        public string ACC_OUTLET { get; set; }
        public string ACC_ZONE { get; set; }
        public string ACC_PASSWORD { get; set; }
        public string ACC_EMPLOYEE_IND { get; set; }
        public string ACC_STATEMENT_TYPE { get; set; }
        public string ACC_PRICE_LEVEL { get; set; }
        public string ACC_DEL_NAME { get; set; }
        public string ACC_DEL_ADDR_1 { get; set; }
        public string ACC_DEL_ADDR_2 { get; set; }
        public string ACC_DEL_ADDR_3 { get; set; }
        public string ACC_DEL_POST_CODE { get; set; }
        public string ACC_ACCOUNT_CLASS { get; set; }
        public DateTime? ACC_DATE_JOINED { get; set; }
        public DateTime? ACC_FINANCIAL_UNTIL { get; set; }
        public DateTime? ACC_SUSPEND_UNTIL { get; set; }
        public string ACC_SUSPEND_REASON { get; set; }
        public string ACC_NOTES_LINE_1 { get; set; }
        public string ACC_NOTES_LINE_2 { get; set; }
        public string ACC_NOTES_LINE_3 { get; set; }
        public string ACC_NOTES_LINE_4 { get; set; }
        public string ACC_NOTES_LINE_5 { get; set; }
        public string ACC_NOTES_LINE_6 { get; set; }
        public string ACC_SEND_PROMO_INFO_IND { get; set; }
        public string ACC_FLAGS { get; set; }
        public string ACC_Last_Modified_Date { get; set; }

        public byte ACC_OWL_CLUB_USER { get; set; }
        public string ACC_ADDR_UNIT_NUMBER { get; set; }
        public string ACC_ADDR_STREET_NUMBER { get; set; }
        public string ACC_ADDR_CITY { get; set; }
        public string ACC_ADDR_STATE { get; set; }
        public byte ACC_DELETED { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores

        public virtual ICollection<JournalDetail> JournalMemberAccount { get; }
    }
}
