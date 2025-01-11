using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace MY.Controls.Docking.Views;
public partial class MainView : UserControl
{
    private bool _isDragging = false;
    private Point _startPoint;
    private Border _draggedPanel;
    private Border _dockIndicator;

    public MainView()
    {
        InitializeComponent();

        var bk = new SolidColorBrush(Colors.Blue) { Opacity = 0.5 };
        // 创建定位框
        _dockIndicator = new Border
        {
            Background = bk,
            BorderBrush = Brushes.Blue,
            BorderThickness = new Thickness(2),
            IsVisible = false
        };

        // 将定位框添加到覆盖层
        canvasContent.Children.Add(_dockIndicator);
        //var content = (this.Content as Control);
        //this.Content = new Grid
        //{
        //    Children = { (this.Content as Control), overlayCanvas }
        //};

        // 监听拖动事件
        AttachDragEvents(LeftPanel);
        AttachDragEvents(RightPanel);
        AttachDragEvents(BottomPanel);
    }

    private void AttachDragEvents(Border panel)
    {
        panel.PointerPressed += OnPointerPressed;
        panel.PointerMoved += OnPointerMoved;
        panel.PointerReleased += OnPointerReleased;
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var currentPoint = e.GetPosition(this);

            // 更新定位框的位置和大小
            UpdateDockIndicator(currentPoint);
        }
    }

    //private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    //{
    //    _isDragging = false;
    //    _dockIndicator.IsVisible = false;

    //    // 处理停靠逻辑
    //    var currentPoint = e.GetPosition(this);
    //    HandleDock(currentPoint);
    //}
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Pointer.Capture(_draggedPanel);
        _isDragging = true;
        _startPoint = e.GetPosition(this);
        _draggedPanel = sender as Border;

        (_draggedPanel.Parent as DockPanel).Children.Remove(_draggedPanel);

        // 创建浮动窗口
        var floatingWindow = new Window
        {
            Content = _draggedPanel,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        floatingWindow.Show();
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);

        // 关闭浮动窗口并重新停靠
        var floatingWindow = (_draggedPanel.Parent as Window);
        floatingWindow.Content = null;
        floatingWindow.Close();

        // 将面板重新添加到主窗口
        var mainGrid = this.Content as Grid;
        mainGrid.Children.Add(_draggedPanel);

        // 处理停靠逻辑
        var currentPoint = e.GetPosition(this);
        HandleDock(currentPoint);
    }

    private void UpdateDockIndicator(Point mousePosition)
    {
        // 根据鼠标位置计算停靠区域
        if (mousePosition.X < this.Bounds.Width / 3)
        {
            // 左侧停靠
            _dockIndicator.Width = 200;
            _dockIndicator.Height = this.Bounds.Height;
            Canvas.SetLeft(_dockIndicator, 0);
            Canvas.SetTop(_dockIndicator, 0);
        }
        else if (mousePosition.X > this.Bounds.Width * 2 / 3)
        {
            // 右侧停靠
            _dockIndicator.Width = 200;
            _dockIndicator.Height = this.Bounds.Height;
            Canvas.SetLeft(_dockIndicator, this.Bounds.Width - 200);
            Canvas.SetTop(_dockIndicator, 0);
        }
        else
        {
            // 中间停靠
            _dockIndicator.Width = this.Bounds.Width - 400;
            _dockIndicator.Height = this.Bounds.Height;
            Canvas.SetLeft(_dockIndicator, 200);
            Canvas.SetTop(_dockIndicator, 0);
        }
    }

    private void HandleDock(Point mousePosition)
    {
        // 根据鼠标位置确定最终的停靠位置
        if (mousePosition.X < this.Bounds.Width / 3)
        {
            // 左侧停靠
            DockPanel.SetDock(_draggedPanel, Dock.Left);
        }
        else if (mousePosition.X > this.Bounds.Width * 2 / 3)
        {
            // 右侧停靠
            DockPanel.SetDock(_draggedPanel, Dock.Right);
        }
        else
        {
            // 中间停靠
            DockPanel.SetDock(_draggedPanel, Dock.Bottom);
        }
    }
}