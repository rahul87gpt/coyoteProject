

// Menu Toggle
//========================================

jQuery(document).ready(function(){

   // Sidebar Menu button
      //========================================

      $('.sidebar-left #sbLeftOpen').on('click', function () {
         $(this).parent('.sidebar-left').toggleClass('sb-close');
         $(this).parent('.sidebar-left').toggleClass('sb-open');
         $('body').toggleClass('sb-open-wrap');
       });
       // $('.sidebar-left.sb-close .proSidebar-menu > li').hover(function () {
       //   $(this).parents('.sidebar-left.sb-close').toggleClass('sbHover-Open');
       //   $(this).parents('.sidebar-left.sb-close').toggleClass('sbHover-close');
       // });
       $(".sidebar-left .proSidebar-menu li").on('click', function () {
           $('li.active').removeClass('active');
           $(this).addClass('active');
       });
       $("ul.submenu li a").on('click', function () {
           $('a.active').removeClass('active');
           $(this).addClass('active');
       });
       $("#sbLeftOpen").on('click', function () {
         if ($('.sidebar-left').hasClass("sb-close")) {
           $('ul.submenu').slideUp();
           $('li.has-submenu').removeClass('submenu-open');
         }
       });
       $('ul.proSidebar-menu > li.has-submenu').on('click', function () {
         $(this).find('ul.submenu').slideToggle();
         $(this).toggleClass('submenu-open');
       });


   // Custom Menu
   //========================================
   $('ul.ctm-menu > li.has-submenu > a').on('click', function(){
      $(this).parent('li.has-submenu').toggleClass('cm-open');
      $(this).siblings('.submenu-wrap').slideToggle();
   });
   
   
//    $(document).click(function (e) {
//       e.stopPropagation();
//       var container = $(".selectMenuCtm");
  
//       //check if the clicked area is dropDown or not
//       if (container.has(e.target).length === 0) {
//          $('ul.ctm-menu.selectMenuCtm > li.has-submenu > a').siblings('.submenu-wrap').hide(500);
//       }
//   })

   // Mobile Menu
   //========================================
   $('.header-wrap .mainMenu-toggle').on('click', function(){
      $(this).parents('.header-wrap').toggleClass('mainMenu-open');
   });

   // end js

});

// End 
//========================================
