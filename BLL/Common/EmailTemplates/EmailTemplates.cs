using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.enums;

namespace E_commerce.BLL.Common.EmailTemplates
{
    public static class EmailTemplates
    {
        public static string OrderEmail(Order order)
        {
            var customerName = order.ApplicationUser?.FirstName ?? "Customer";
            var statusMessage = order.OrderStatus switch
            {
                OrderStatus.Pending =>
                    "We've received your order and it's waiting to be confirmed.",

                OrderStatus.Approved =>
                    "Great news! Your order has been approved and is now being prepared.",

                OrderStatus.Processing =>
                    "Your order is currently being processed. We'll notify you when it ships.",

                OrderStatus.Shipped =>
                    "Your order has been shipped! You can track it using the information below.",

                OrderStatus.Delivered =>
                    "Your order has been delivered successfully. We hope you enjoy your purchase!",

                OrderStatus.Cancelled =>
                    "Unfortunately, your order has been cancelled. If you have any questions, please contact our support team.",

                _ =>
                    "Your order status has been updated."
            };

            var headerColor = order.OrderStatus switch
            {
                OrderStatus.Pending => "#f39c12",
                OrderStatus.Approved => "#3498db",
                OrderStatus.Processing => "#9b59b6",
                OrderStatus.Shipped => "#16a085",
                OrderStatus.Delivered => "#2ecc71",
                OrderStatus.Cancelled => "#e74c3c",
                _ => "#3498db"
            };

            var headerTitle = order.OrderStatus switch
            {
                OrderStatus.Pending => "Order Received",
                OrderStatus.Approved => "Order Approved",
                OrderStatus.Processing => "Order Processing",
                OrderStatus.Shipped => "Order Shipped",
                OrderStatus.Delivered => "Order Delivered",
                OrderStatus.Cancelled => "Order Cancelled",
                _ => "Order Update"
            };

            var shippingSection = string.Empty;

            if ((order.OrderStatus == OrderStatus.Shipped ||
                 order.OrderStatus == OrderStatus.Delivered) &&
                (!string.IsNullOrWhiteSpace(order.Carrier) ||
                 !string.IsNullOrWhiteSpace(order.TrackingNumber)))
            {
                shippingSection = $@"
                <div class='box'>
                    <h3>Shipping Information</h3>

                    <table>
                        <tr>
                            <td class='label'>Shipping Company</td>
                            <td class='value'>{order.Carrier}</td>
                        </tr>

                        <tr>
                            <td class='label'>Tracking Number</td>
                            <td class='value'>{order.TrackingNumber}</td>
                        </tr>

                        <tr>
                            <td class='label'>Shipping Date</td>
                            <td class='value'>{order.ShippingDate:MMMM dd, yyyy}</td>
                        </tr>
                    </table>
                </div>";
            }

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>

<meta charset='UTF-8'>

<style>

body {{
    margin:0;
    padding:0;
    background:#f4f6f8;
    font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;
}}

.container {{
    max-width:650px;
    margin:40px auto;
    background:#fff;
    border-radius:12px;
    overflow:hidden;
    box-shadow:0 5px 18px rgba(0,0,0,.08);
}}

.header {{
    background:{headerColor};
    color:#fff;
    text-align:center;
    padding:35px;
}}

.header h1 {{
    margin:0;
    font-size:28px;
}}

.content {{
    padding:35px;
    color:#333;
}}

.content p {{
    line-height:1.8;
}}

.status {{
    display:inline-block;
    background:{headerColor};
    color:white;
    padding:8px 18px;
    border-radius:25px;
    font-weight:bold;
    margin:15px 0;
}}

.box {{
    margin-top:25px;
    border:1px solid #e5e7eb;
    border-radius:8px;
    padding:20px;
    background:#fafafa;
}}

.box h3 {{
    margin-top:0;
    margin-bottom:15px;
}}

table {{
    width:100%;
    border-collapse:collapse;
}}

td {{
    padding:10px 0;
}}

.label {{
    color:#666;
}}

.value {{
    text-align:right;
    font-weight:bold;
    color:#111;
}}

.footer {{
    background:#fafafa;
    color:#888;
    text-align:center;
    padding:20px;
    font-size:12px;
}}

</style>

</head>

<body>

<div class='container'>

    <div class='header'>
        <h1>{headerTitle}</h1>
    </div>

    <div class='content'>

        <p>
            Hello <strong>{customerName}</strong>,
        </p>

        <p>
            {statusMessage}
        </p>

        <span class='status'>
            {order.OrderStatus}
        </span>

        <div class='box'>

            <h3>Order Details</h3>

            <table>

                <tr>
                    <td class='label'>Order Number</td>
                    <td class='value'>#{order.Id}</td>
                </tr>

                <tr>
                    <td class='label'>Order Date</td>
                    <td class='value'>{order.OrderDate:MMMM dd, yyyy}</td>
                </tr>

                <tr>
                    <td class='label'>Payment Status</td>
                    <td class='value'>{order.PaymentStatus}</td>
                </tr>

                <tr>
                    <td class='label'>Order Total</td>
                    <td class='value'>{order.OrderTotal:C}</td>
                </tr>

            </table>

        </div>

        {shippingSection}

    </div>

    <div class='footer'>
        © {DateTime.Now.Year} E-Commerce. All Rights Reserved.
    </div>

</div>

</body>

</html>";
        }
    }
}