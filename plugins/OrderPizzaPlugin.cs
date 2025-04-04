using Microsoft.SemanticKernel;

public class OrderPizzaPlugin
{
    private string _size = "";
    private List<string> _toppings = new();
    private bool _orderConfirmed = false;

    [KernelFunction("select_pizza_size")]
    public Task<string> SelectPizzaSize(string size)
    {
        _size = size;
        return Task.FromResult($"Pizza size set to {size}. What toppings would you like?");
    }

    [KernelFunction("add_toppings")]
    public Task<string> AddToppings(string toppings)
    {
        _toppings.AddRange(toppings.Split(", "));
        return Task.FromResult($"Added toppings: {string.Join(", ", _toppings)}. Do you want to confirm your order?");
    }

    [KernelFunction("confirm_order")]
    public Task<string> ConfirmOrder()
    {
        _orderConfirmed = true;
        return Task.FromResult($"Your order: {_size} pizza with {string.Join(", ", _toppings)}. Type 'place order' to finalize.");
    }

    [KernelFunction("place_order")]
    public Task<string> PlaceOrder()
    {
        if (!_orderConfirmed)
        {
            return Task.FromResult("Please confirm your order before placing it.");
        }

        string orderDetails = $"Order placed: {_size} pizza with {string.Join(", ", _toppings)}. Enjoy your meal!";
        _size = "";
        _toppings.Clear();
        _orderConfirmed = false;
        return Task.FromResult(orderDetails);
    }
}