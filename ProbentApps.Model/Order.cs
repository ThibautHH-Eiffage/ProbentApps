using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProbentApps.Model;

public class Order
{
    public Guid Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Affair Affair { get; set; } = default!;

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
    public Client Client { get; set; } = default!;

    [MaxLength(64)]
    [Unicode(false)]
    public string Code { get; set; } = default!;

    [Precision(38, 2)]
    public decimal TotalPrice { get; set; }

    public IList<Advancement> Advancements { get; set; } = [];

    public IList<Report> Reports { get; set; } = [];

    public IList<Invoice> Invoices { get; set; } = [];
}
