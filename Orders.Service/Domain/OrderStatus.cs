namespace Orders.Service.Domain;

// статусы заказа - только созданный, оплаченный, статус ошибки/отмены
public enum OrderStatus
{
    New,
    Paid,
    Failed
}