﻿using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * This is not a well thought out class, but it contains several methods that
 * useful for finding loan files last modified between certain dates.
 * 
 * Per the docs - Loans.Query is the fastest and least resource intensive, but
 * only returns guids
 * 
 * Docs say that pipeline SHOULD be faster than querying, but on GR's hardware, 
 * for THESE queries, reporting is 3-4x pipeline
 * 
 */

namespace GuaranteedRate.Sextant.EncompassUtils
{
    public class Reporting
    {
        private const string LAST_MODIFIED_FIELD = "LASTMODIFIED";

        /**
         * Returns an IList of loan guids last modified between the start and end date
         * NOTE: There is NO sorting implied in the list
         */

        public static IList<string> LoansLastModifiedBetween(Session session, DateTime start)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            return LoansLastModifiedBetween(session, startDate);
        }

        public static IList<string> LoansLastModifiedBetween(Session session, DateTime start, DateTime end)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            DateFieldCriterion endDate = new DateFieldCriterion();
            endDate.FieldName = LAST_MODIFIED_FIELD;
            endDate.MatchType = OrdinalFieldMatchType.LessThan;
            endDate.Value = end;
            endDate.Precision = DateFieldMatchPrecision.Day;

            QueryCriterion joinCriterion = startDate.And(endDate);
            return LoansLastModifiedBetween(session, joinCriterion);
        }

        public static IList<string> LoansLastModifiedBetween(Session session, QueryCriterion criterion)
        {
            IList<string> loans = new List<string>();

            try
            {
                LoanIdentityList loanList = session.Loans.Query(criterion);
                foreach (LoanIdentity id in loanList)
                {
                    loans.Add(id.Guid);
                }
            }
            catch
            {
                //noop
            }
            return loans;
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedPipeline(Session session, DateTime start)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            return LoansAndLastModifiedPipeline(session, startDate);
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedPipeline(Session session, DateTime start,
            DateTime end)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            DateFieldCriterion endDate = new DateFieldCriterion();
            endDate.FieldName = LAST_MODIFIED_FIELD;
            endDate.MatchType = OrdinalFieldMatchType.LessThan;
            endDate.Value = end;
            endDate.Precision = DateFieldMatchPrecision.Day;

            QueryCriterion joinCriterion = startDate.And(endDate);
            return LoansAndLastModifiedPipeline(session, joinCriterion);
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedPipeline(Session session,
            QueryCriterion criterion)
        {
            IDictionary<string, DateTime> loans = new Dictionary<string, DateTime>();
            try
            {
                PipelineCursor cursor = session.Loans.QueryPipeline(criterion, PipelineSortOrder.None);
                foreach (PipelineData p in cursor)
                {
                    loans.Add(p.LoanIdentity.Guid, (DateTime) p["LastModified"]);
                }
                cursor.Close();
            }
            catch
            {
                //noop
            }
            return loans;
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedReport(Session session, DateTime start)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            return LoansAndLastModifiedReport(session, startDate);
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedReport(Session session, DateTime start,
            DateTime end)
        {
            DateFieldCriterion startDate = new DateFieldCriterion();
            startDate.FieldName = LAST_MODIFIED_FIELD;
            startDate.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;
            startDate.Value = start;
            startDate.Precision = DateFieldMatchPrecision.Day;

            DateFieldCriterion endDate = new DateFieldCriterion();
            endDate.FieldName = LAST_MODIFIED_FIELD;
            endDate.MatchType = OrdinalFieldMatchType.LessThan;
            endDate.Value = end;
            endDate.Precision = DateFieldMatchPrecision.Day;

            QueryCriterion joinCriterion = startDate.And(endDate);

            return LoansAndLastModifiedReport(session, joinCriterion);
        }

        public static IDictionary<string, DateTime> LoansAndLastModifiedReport(Session session, QueryCriterion criterion)
        {
            IDictionary<string, DateTime> loans = new Dictionary<string, DateTime>();
            StringList fields = new StringList();
            fields.Add("Loan.LastModified");

            try
            {
                LoanReportCursor results = session.Reports.OpenReportCursor(fields, criterion);
                foreach (LoanReportData d in results)
                {
                    loans.Add(d.Guid, (DateTime) d["Loan.LastModified"]);
                }
                results.Close();
            }
            catch
            {
                //noop
            }
            return loans;
        }
    }
}