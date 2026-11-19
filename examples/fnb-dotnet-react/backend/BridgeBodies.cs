namespace SofiPOS.FnB.Demo;

public sealed class QrOrderCodesSetActiveBody
{
    public bool IsActive { get; set; }
}

public sealed class PricingPoliciesGetProductAddOnsBody
{
    public List<Guid> ProductIds { get; set; } = new();
}

public sealed class VouchersSetActiveBody
{
    public bool IsActive { get; set; }
}

public sealed class FnbOrdersUpdateNoteBody
{
    public string? Note { get; set; }
}

public sealed class FnbOrdersSetCustomerBody
{
    public Guid? CustomerId { get; set; }
}

public sealed class FnbOrdersCancelBody
{
    public string CancelReason { get; set; } = string.Empty;
}

public sealed class FnbOrdersUpdateProductQuantityBody
{
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
}

public sealed class FnbOrdersUpdateProductUnitBody
{
    public Guid? UnitId { get; set; }
}

public sealed class FnbOrdersUpdateProductPriceBody
{
    public decimal Price { get; set; }
    public string? Reason { get; set; }
}

public sealed class FnbOrdersUpdateProductNoteBody
{
    public string? Note { get; set; }
}

public sealed class FnbOrdersCancelProductBody
{
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public sealed class FnbOrdersCancelComboBody
{
    public string Reason { get; set; } = string.Empty;
}

public sealed class FnbOrdersChangeTableBody
{
    public Guid? TableId { get; set; }
    public string? SaleChannelId { get; set; }
}

public sealed class FnbOrdersMergeBody
{
    public Guid SourceOrderId { get; set; }
    public Guid TargetOrderId { get; set; }
}
