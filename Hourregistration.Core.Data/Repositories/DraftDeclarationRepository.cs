using System;
using System.Collections.Generic;
using System.Linq;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class DraftDeclarationRepository : IDraftDeclarationRepository
    {
        // keep internal list of drafts
        private readonly List<DeclaredHours> draftDeclarations = new();

        // Add a draft: store a copy to avoid callers later mutating the same instance
        public DeclaredHours AddDraft(DeclaredHours declaration)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));

            // clone into a new DeclaredHours instance so callers can't later mutate the stored draft
            var copy = new DeclaredHours(
                declaration.Id,
                declaration.Date,
                (int)declaration.WorkedHours,
                declaration.Reason ?? string.Empty,
                declaration.Description ?? string.Empty,
                declaration.UserId)
            {
                State = declaration.State,
                CreatedAt = declaration.CreatedAt,
                UpdatedAt = declaration.UpdatedAt,
                User = declaration.User
            };

            draftDeclarations.Add(copy);
            return copy;
        }

        // Return all drafts (shallow copy of list)
        public List<DeclaredHours> GetAllDrafts()
        {
            return draftDeclarations.ToList();
        }

        // Delete a draft by Id (not by reference) — callers may pass a different instance with same values
        // Return the removed draft or null when not found
        public DeclaredHours? DeleteDraft(DeclaredHours declaration)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));

            // find by Id first (preferred). If Id == 0 fall back to value match.
            DeclaredHours? existing = null;
            if (declaration.Id != 0)
            {
                existing = draftDeclarations.FirstOrDefault(d => d.Id == declaration.Id);
            }

            if (existing == null)
            {
                // try to match by content (date+hours+reason+description+user) as fallback
                existing = draftDeclarations.FirstOrDefault(d =>
                    d.Date == declaration.Date
                    && Math.Abs(d.WorkedHours - declaration.WorkedHours) < 0.0001
                    && string.Equals(d.Reason ?? string.Empty, declaration.Reason ?? string.Empty, StringComparison.Ordinal)
                    && string.Equals(d.Description ?? string.Empty, declaration.Description ?? string.Empty, StringComparison.Ordinal)
                    && d.UserId == declaration.UserId);
            }

            if (existing == null)
            {
                // not found — return null instead of throwing
                return null;
            }

            draftDeclarations.Remove(existing);
            return existing;
        }
    }
}