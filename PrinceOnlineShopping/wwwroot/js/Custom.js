
    function liClick(id) {
    $.post("/AddToCart/Add", {
        "product": id
    },
        function (data) {
            $("#tableViewCart").empty();
            var totaldiscountPrice = 0;
            var totalcount = Object.keys(data).length;
            $(".totalcount").html(totalcount);
            //if (totalcount > 2) {
            //    var ul1 = '<tr><td><span class="more2Product"></span></td></tr>';
            //    var ul = $.parseHTML(ul1);
            //    $(".more2Product", ul).html(totalcount + ' Products are added in cart.Please visit cart for more Details');
            //    $("#tableViewCart").append(ul);
            //} else {
            //    $.each(data, function (i, item) {
            //        var ul1 = '<tr> <td class="si-pic"><span class="imageclass"></span></td> <td class="si-text"> <div class="product-selected"> <p> <span class="cartpriceclass"></span></p> <h6> <span class="cartdescclass"></span></h6> </div> </td> <td class="si-close"> <span class="cartremocveclass"> </span> </td> </tr>';
            //        var ul = $.parseHTML(ul1)
            //        $(".imageclass", ul).html(' <img  src="../images/' + item.productImage[0].fileName + '" alt="" style="height:100px; width:100px">');
            //        console.log(item.productImage[0].fileName + item.discountPrice + item.productId)
            //        $(".cartpriceclass", ul).html(item.discountPrice);
            //        $(".cartdescclass", ul).html(item.shortDescription);
            //        $(".cartremocveclass", ul).html('<a asp-action="Remove" asp-controller="AddToCart" asp-route-product="' + item.productId + '"><i class="ti-close"></i> </a>');
            //        $("#tableViewCart").append(ul);
            //    });
            //}
            if (totalcount > 0) {
                $.each(data, function (i, item) {
                    totaldiscountPrice += item.discountPrice
                });
                $(".carttotalprice").html('₹' + totaldiscountPrice.toFixed(2));
                $(".totalcartprice").html('₹' + totaldiscountPrice.toFixed(1));
            }

            SuccessMessage();
        }
    );
}

function SuccessMessage() {
    var x = document.getElementById("snackbar");
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}