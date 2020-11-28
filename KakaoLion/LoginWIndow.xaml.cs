﻿using System;
using System.Collections.Generic;
using System.Windows;
using KakaoLion.database.repository;
using KakaoLion.database.repositoryImpl;
using KakaoLion.model;
using KakaoLion.server.repository;
using KakaoLion.server.repositoryImpl;

namespace KakaoLion
{
    public partial class LoginWindow : Window
    {
        private bool isCheck = false;
        private List<UserModel> userList = new List<UserModel>();

        private LoginMessageRepository loginRepository;
        private UserRepository userRepository;

        public LoginWindow()
        {
            InitializeComponent();

            loginRepository = new LoginMessageRepositoryImpl();
            userRepository = new UserRepositoryImpl();

            checkAutoLogin();
        }

        private void checkAutoLogin()
        {
            bool isAutoLogin = Properties.Settings.Default.isAutoLogin;
            if (isAutoLogin)
            {
                string userId = Properties.Settings.Default.userId;

                if (App.isRunning)
                {
                    loginRepository.sendLoginMessage(userId);
                }
                else
                {
                    checkReconnectServer();
                }
                App.userId = userId;

                MainWindow MainWindow = new MainWindow();
                MainWindow.Show();
                this.Close();
            } 
            else
            {
                getAllUser();
            }
        }

        private void getAllUser()
        {
            userList.Clear();
            userList = userRepository.getAllUser();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string userId = "";
            bool isSuccess = false;

            foreach (UserModel user in userList)
            {
                if (user.id == textBoxId.Text.ToString() && user.pw == textBoxPw.Text.ToString())
                {
                    userId = user.id;
                    isSuccess = true;
                    break;
                }
            }

            if (isSuccess)
            {
                if (isCheck)
                {
                    Properties.Settings.Default.isAutoLogin = true;
                }

                Properties.Settings.Default.userId = userId;
                Properties.Settings.Default.Save();

                if (App.isRunning)
                {
                    loginRepository.sendLoginMessage(userId);
                }
                else
                {
                    checkReconnectServer();
                }
                App.userId = userId;

                MainWindow MainWindow = new MainWindow();
                MainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("로그인 정보가 올바르지 않습니다.", "KAKAO");
            }
        }

        private void checkReconnectServer()
        {
            while (true)
            {
                if (MessageBox.Show("서버와 연결이 유실되었습니다.\n(서버와 재연결을 하시겠습니까?)", "서버 재연결", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    App.connectServer();

                    if (App.isRunning)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isCheck = true;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isCheck = false;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            App.LoginWindow_CloseAction(true);
        }
    }
}
