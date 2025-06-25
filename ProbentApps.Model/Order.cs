using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProbentApps.Model;

public class Order
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Affair Affair { get; set; }

    /// <summary>
    /// The order's <see cref="Client"/>, as imported from the external provider.
    /// </summary>
    /// <remarks>
    /// Conceptually corresponds to the <see cref="Model.Client"/> that will be invoiced.
    /// An <see cref="Invoice"/> should never contain <see cref="Invoice.Advancements"/>
    /// for an <see cref="Advancement.Order"/> that has a <see cref="Client"/> different
    /// from another <see cref="Invoice.Advancements"/>' <see cref="Advancement.Order"/>
    /// <see cref="Client"/>.
    /// </remarks>
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public required Client Client { get; set; }

    [MaxLength(64)]
    public required string ProviderId { get; set; }

    public required IList<Advancement> Advancements { get; set; }

    public required IList<Report> Reports { get; set; }

    public required IList<Invoice> Invoices { get; set; }
}
