$(document).ready(function () {

  var header_height = $('.master-header').height();
  $('.master-header').wrap('<div class="navbar-wrapper"></div>');
  if ($(window).width() > 990) {
    $('.navbar-wrapper').css('height', header_height);
  } else {
    var wrapper_height = $('.master-header').height() + $('.search-box').height();
    $('.navbar-wrapper').css('height', wrapper_height);
  }

  $('.home-page-category-grid .item-grid').owlCarousel({
    items: 5,
    margin: 15,
    autoplay: true,
    nav: true,
    loop: true,
    autoplayTimeout: 3000,
    responsiveClass: true,
    responsive: {
      0: {
        items: 1
      },
      576: {
        items: 2
      },
      768: {
        items: 3
      },
      1200: {
        items: 3
      },
      1400: {
        items: 4
      }

    }
  });

  $('#homepage-promotion').owlCarousel({
    animateOut: 'fadeOut',
    animateIn: 'fadeIn',
    items: 1,
    margin: 15,
    loop: true,
    autoplay: true,
    autoplayTimeout: 8000,
     responsive: {
      0: {
        items: 1
      },
      576: {
        items: 2
      },
      768: {
        items: 3
      },
      1200: {
        items: 3
      },
      1400: {
        items: 4
      }

    }
  });

  $('.related-products-grid .item-grid,.also-purchased-products-grid .item-grid').owlCarousel({
    items: 4,
    margin: 14,
    loop: false,
    autoplay: true,
    autoplayTimeout: 8000,
    nav: true,
    responsive: {
      0: {
        items: 1
      },
      576: {
        items: 2
      },
      768: {
        items: 3
      },
      1200: {
        items: 3
      },
      1300: {
        items: 4
      },
      1400: {
        items: 4
      }

    }
  });

  $('#picture-thumbs').owlCarousel({
    items: 5,
    margin: 15,
    loop: true
  });

  if ($(window).width() < 992) {
    $('.side-2 button[data-toggle="collapse"]').addClass('collapsed');
    $('.collapse.show').removeClass('show')
  };

  $('select:not([class])').addClass('form-control');

    $(window).on('load resize', function () {
        if ($(window).width() > 991) {
            var c, currentScrollTop = 0

            //for headerlink dropdown position
            $('.header-links .nav-item.dropdown').each(function () {
                const navLeftPosition = $(this).offset().left;
                const viewportWidth = $(document).width();
                const haveToPosition = viewportWidth - navLeftPosition - $(this).width() - 10;
                const rightPosition = "-" + haveToPosition + "px";
                const dropdownMenu = $(this).children('.dropdown-menu');
                if (dropdownMenu.width() >= haveToPosition) {
                    dropdownMenu.css({
                        "left": "initial",
                        "right": rightPosition
                    })
                } else {
                    dropdownMenu.removeAttr("style");
                }
            })

        } else {
            var header_height = $('.master-header').height();
            $('.top-menu').css({
                'top': header_height,
                'height': ($(window).height() - header_height),
                'transform': 'translateX(-120%)'
            })

        }
    })

});

//scroll to top
$(window).scroll(function () {
  if ($(this).scrollTop() > 100) {
    $('.scrollup').fadeIn();
  } else {
    $('.scrollup').fadeOut();
  }
});
$('.scrollup').click(function () {
  $("html, body").animate({ scrollTop: 0 }, 1000);
  return false;
});
