document.addEventListener('DOMContentLoaded', function() {
  // expand/collapse
  document.querySelectorAll('.panel-item .item-summary').forEach(function(summary) {
    summary.addEventListener('click', function() {
      var panelItem = this.closest('.panel-item');
      panelItem.classList.toggle('open');
      var caret = this.querySelector('.item-expand');
      if (panelItem.classList.contains('open')) caret.textContent = '▴'; else caret.textContent = '▾';
    });
  });

  // filtering by status
  var statusButtons = document.querySelectorAll('.filter-btn[data-status]');
  statusButtons.forEach(function(btn) {
    btn.addEventListener('click', function() {
      statusButtons.forEach(b=>b.classList.remove('active'));
      btn.classList.add('active');
      var status = btn.getAttribute('data-status');
      filterByStatus(status);
    });
  });

  function filterByStatus(status) {
    document.querySelectorAll('.panel-item').forEach(function(item) {
      var statusTag = item.querySelector('.tag.status');
      var reviewStatus = statusTag ? statusTag.textContent.trim() : 'Pending';
      // normalize possible values (case-insensitive)
      var normalized = reviewStatus.toLowerCase();
      if (status === 'All' || normalized === status.toLowerCase()) item.style.display = '';
      else item.style.display = 'none';
    });
  }

  // date filtering
  document.getElementById('date-filter-btn').addEventListener('click', function() {
    var from = document.getElementById('date-from').value;
    var to = document.getElementById('date-to').value;
    filterByDate(from, to);
  });

  function filterByDate(from, to) {
    var fromTime = from ? new Date(from).getTime() : null;
    var toTime = to ? new Date(to).getTime() : null;
    document.querySelectorAll('.panel-item').forEach(function(item) {
      var dateTag = item.querySelector('.tag.date');
      var dateStr = dateTag ? dateTag.textContent.trim() : null;
      var dateTime = dateStr ? new Date(dateStr).getTime() : null;
      var show = true;
      if (fromTime && dateTime && dateTime < fromTime) show = false;
      if (toTime && dateTime && dateTime > toTime) show = false;
      item.style.display = show ? '' : 'none';
    });
  }

});