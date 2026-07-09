namespace Ecommerce.Utility
{
    public static class EmailTemplateBuilder
    {
        public static string BuildActionEmail(
            string title,
            string message,
            string? buttonText = null,
            string? actionLink = null)
        {
            return $@"
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f4f4f4;padding:40px 0;'>
    <tr>
        <td align='center'>
            <table width='520' cellpadding='0' cellspacing='0' border='0'
                   style='background-color:#ffffff;border-radius:10px;padding:30px;'>

                <tr>
                    <td align='center'>

                        <h1 style='margin:0 0 20px 0;color:#222;font-family:Arial,sans-serif;'>
                            {title}
                        </h1>

                        <p style='margin:0 0 25px 0;color:#555;font-size:16px;line-height:24px;font-family:Arial,sans-serif;'>
                            {message}
                        </p>

                        <a href='{actionLink}'
                           style='background-color:#198754;color:#fff;text-decoration:none;
                                  padding:12px 24px;border-radius:6px;display:inline-block;
                                  font-family:Arial,sans-serif;font-weight:bold;'>
                            {buttonText}
                        </a>

                    </td>
                </tr>

            </table>
        </td>
    </tr>
</table>";
        }
    }
}
