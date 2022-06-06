function rateProduct(el){
	let productId = el.dataset.productId;
$.ajax({
	url: "/Product/Rate",
	method: "POST",
	data: {
		"id":productId,
		"rating":el.rating.value
		},
	success: function (response){
		$(`#avgRating[data-product-id=${productId}]`)[0].textContent = response;
		el.rating.value = Math.round(response);
		} 
	});
}
