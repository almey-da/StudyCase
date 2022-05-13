namespace ProductServcie.GraphQL
{
    public record ProductInput
    (
        int? Id,
        string Name,
        int Stock,
        double Price
    );
}
