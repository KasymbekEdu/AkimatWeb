// ── Mobile nav toggle
document.addEventListener('DOMContentLoaded', function () {
  const toggle = document.getElementById('navToggle');
  const nav    = document.getElementById('mainNav');
  if (toggle && nav) {
    toggle.addEventListener('click', () => nav.classList.toggle('open'));
    document.addEventListener('click', e => {
      if (!toggle.contains(e.target) && !nav.contains(e.target))
        nav.classList.remove('open');
    });
  }

  // ── Modals
  document.querySelectorAll('[data-modal]').forEach(btn => {
    btn.addEventListener('click', () => {
      const id = btn.dataset.modal;
      document.getElementById(id)?.classList.add('open');
    });
  });
  document.querySelectorAll('.modal-overlay').forEach(overlay => {
    overlay.addEventListener('click', e => {
      if (e.target === overlay) overlay.classList.remove('open');
    });
  });
  document.querySelectorAll('.modal-close').forEach(btn => {
    btn.addEventListener('click', () => {
      btn.closest('.modal-overlay')?.classList.remove('open');
    });
  });

  // ── Auto-dismiss alerts
  document.querySelectorAll('.alert[data-autohide]').forEach(el => {
    setTimeout(() => el.remove(), 4000);
  });

  // ── Confirm delete
  document.querySelectorAll('[data-confirm]').forEach(btn => {
    btn.addEventListener('click', e => {
      if (!confirm(btn.dataset.confirm || 'Жойғыңыз келеді ме?'))
        e.preventDefault();
    });
  });

  // ── Phone mask: +7 (___) ___-__-__
  document.querySelectorAll('input[type="tel"]').forEach(input => {
    input.addEventListener('input', function () {
      let val = this.value.replace(/\D/g, '');
      if (val.startsWith('8')) val = '7' + val.slice(1);
      if (!val.startsWith('7')) val = '7' + val;
      val = val.slice(0, 11);
      let masked = '+7';
      if (val.length > 1) masked += ' (' + val.slice(1, 4);
      if (val.length >= 4) masked += ') ' + val.slice(4, 7);
      if (val.length >= 7) masked += '-' + val.slice(7, 9);
      if (val.length >= 9) masked += '-' + val.slice(9, 11);
      this.value = masked;
    });
  });

  // ── Tracking number uppercase
  const trackInput = document.querySelector('input[name="number"]');
  if (trackInput) trackInput.addEventListener('input', function () {
    this.value = this.value.toUpperCase();
  });
});
