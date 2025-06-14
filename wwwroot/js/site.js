document.addEventListener('DOMContentLoaded', function() {
    // Initialize all animations and effects
    initializeSlideshow();
    initializeCounters();
    initializeParallax();
    initializeAOS();
    handleScrollEffects();
});

function initializeSlideshow() {
    const slides = document.querySelectorAll('.slide');
    let currentSlide = 0;
    
    function showSlide(index) {
        slides.forEach(slide => slide.classList.remove('active'));
        slides[index].classList.add('active');
    }

    function nextSlide() {
        currentSlide = (currentSlide + 1) % slides.length;
        showSlide(currentSlide);
    }

    slides.forEach(slide => {
        const bgUrl = slide.dataset.image;
        const img = new Image();
        img.src = bgUrl;
        img.onload = () => {
            slide.style.backgroundImage = `url(${bgUrl})`;
            slide.classList.add('loaded');
        };
    });

    if (slides.length > 0) {
        showSlide(0);
        setInterval(nextSlide, 5000);
    }
}

function initializeCounters() {
    const counters = document.querySelectorAll('.counter');
    const options = {
        threshold: 0.5,
        once: true
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const counter = entry.target;
                const target = parseInt(counter.dataset.target);
                animateCounter(counter, target);
                observer.unobserve(counter);
            }
        });
    }, options);

    counters.forEach(counter => observer.observe(counter));
}

function animateCounter(counter, target) {
    const duration = 2000;
    const steps = 100;
    const stepValue = target / steps;
    let current = 0;
    
    const updateCounter = () => {
        current += stepValue;
        if (current < target) {
            counter.textContent = Math.floor(current);
            requestAnimationFrame(updateCounter);
        } else {
            counter.textContent = target;
        }
    };
    
    updateCounter();
}

function initializeParallax() {
    const parallaxElements = document.querySelectorAll('.parallax-window');
    parallaxElements.forEach(element => {
        new Parallax(element, {
            speed: 0.5,
            center: true
        });
    });
}

function initializeAOS() {
    AOS.init({
        duration: 1000,
        once: true,
        offset: 100
    });
}

function handleScrollEffects() {
    const navbar = document.querySelector('.navbar');
    const scrollTopBtn = document.querySelector('.scroll-top');
    
    window.addEventListener('scroll', () => {
        if (window.scrollY > 100) {
            navbar?.classList.add('navbar-scrolled');
            scrollTopBtn?.classList.add('show');
        } else {
            navbar?.classList.remove('navbar-scrolled');
            scrollTopBtn?.classList.remove('show');
        }
    });
}

function addToCart(id, name, price) {
    const button = event.target;
    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Adding...';

    $.ajax({
        url: '/Cart/Add',
        type: 'POST',
        data: { id: id, name: name, price: price },
        success: function(response) {
            if (response.success) {
                updateCartCount(response.cartCount);
                showSuccessMessage(response.message);
            } else {
                showLoginPrompt(response.message);
            }
        },
        error: function() {
            showErrorMessage('Something went wrong. Please try again.');
        },
        complete: function() {
            button.disabled = false;
            button.innerHTML = '<i class="fas fa-shopping-cart"></i> Add to Cart';
        }
    });
}

function updateCartCount(count) {
    const cartCount = document.querySelector('.cart-count');
    if (cartCount) {
        cartCount.textContent = count;
        cartCount.classList.add('pulse');
        setTimeout(() => cartCount.classList.remove('pulse'), 1000);
    }
}

function showSuccessMessage(message) {
    Swal.fire({
        icon: 'success',
        title: 'Added to Cart!',
        text: message,
        timer: 2000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end'
    });
}

function showLoginPrompt(message) {
    Swal.fire({
        icon: 'warning',
        title: 'Login Required',
        text: message,
        showCancelButton: true,
        confirmButtonText: 'Login Now',
        cancelButtonText: 'Cancel',
        customClass: {
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-secondary'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/Account/Login';
        }
    });
}

function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        confirmButtonText: 'Try Again'
    });
}
$('.add-to-cart-btn').click(function() {
    const card = $(this).closest('.menu-card');
    const name = card.find('.menu-title').text();
    const price = parseInt(card.find('.menu-price').text().replace('₹', ''));
    const id = Date.now();

    // Add to cart array
    const existingItem = cart.find(item => item.name === name);
    if (existingItem) {
        existingItem.quantity++;
    } else {
        cart.push({ id, name, price, quantity: 1 });
    }

    // Update cart display and open sidebar
    updateCart();
    openCart();
});
