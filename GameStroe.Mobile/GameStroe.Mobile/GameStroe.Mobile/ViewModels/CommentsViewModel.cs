using GameStroe.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GameStroe.Mobile.ViewModels
{
    class CommentsViewModel : ContentPage
    {
        public CommentsViewModel(List<Comment> comments)
        {
            var listView = new ListView
            {
                ItemsSource = comments.ToArray(),

                ItemTemplate = new DataTemplate(typeof(TextCell)),
            };

            listView.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");
            listView.ItemTemplate.SetBinding(TextCell.DetailProperty, "Body");

            Content = new StackLayout
            {
                Children = {
                    listView
                },
            };
        }
    }
}
