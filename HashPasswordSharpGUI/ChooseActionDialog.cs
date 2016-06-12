﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using JaSt.HashPasswordSharp.Properties;

namespace JaSt.HashPasswordSharp
{
    public partial class ChooseActionDialog : Form
    {
        private readonly string _password;

        public ChooseActionDialog(string password)
        {
            InitializeComponent();
            _password = password;
            txtPassword.Text = password;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnShow.Text = txtPassword.UseSystemPasswordChar ? Resources.ChooseActionDialog_btnShowPassword : Resources.ChooseActionDialog_btnHidePassword;
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_password);
        }

        private void btnShowQRCode_Click(object sender, EventArgs e)
        {
            var qrGenerator = new QrEncoder();
            var qrCode = qrGenerator.Encode(_password);

            var gRenderer = new GraphicsRenderer(
                new FixedModuleSize(6, QuietZoneModules.Two));

            using (var ms = new MemoryStream())
            {
                gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                using (pbQrCode.Image)
                {
                    pbQrCode.Image = new Bitmap(ms);
                }
            }

        }

        private void ChooseActionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (pbQrCode.Image)
            {
                pbQrCode.Image = null;
            }
        }
    }
}
