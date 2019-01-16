using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Nethereum.Web3;
using Nethereum.Geth;
using Nethereum.Signer;
using Nethereum.Hex.HexTypes;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace BayroWallet
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource _cancel;
        private Wallet wallet;

        public MainPage()
        {
            InitializeComponent();

            btnCreateAccount.Clicked += BtnCreateAccount_Clicked;
            btnTransfer.Clicked += BtnTransfer_Clicked;
            btnSync.Clicked += BtnSync_Clicked;
        }

        private async void BtnTransfer_Clicked(object sender, EventArgs e)
        {
            await SendTransaction();

            await DisplayAlert("WhiteAppleby Capital Wallet", "Succesful transaction!", "Ok");
        }

        private async void BtnSync_Clicked(object sender, EventArgs e)
        {
            //await SyncAccount();

            await DisplayAlert("WhiteAppleby Capital Wallet", "The account was synchronized!", "Ok");
        }


        private async void BtnCreateAccount_Clicked(object sender, EventArgs e)
        {
            await AuthenticationAsync("Put your finger!");
        }

        private async Task AuthenticationAsync(string reason, string cancel = null, string fallback = null) //, string tooFast = null)
        {
            _cancel = new CancellationTokenSource();

            var dialogConfig = new AuthenticationRequestConfiguration(reason)
            { // all optional
                CancelTitle = cancel,
                FallbackTitle = fallback,
                AllowAlternativeAuthentication = false
            };

            txtAddress.Text = "";
            lblBalance.Text = "";
            lblUpdatedAt.Text = "";

            var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

            await LoadOrCreateAccount(result);
        }

        private async Task LoadOrCreateAccount(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                try
                {
                    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wallet.json");
                    wallet = Wallet.LoadAccount(fileName);
                    if (wallet == null)
                    {
                        wallet = await Wallet.CreateAccount(fileName);
                        await DisplayAlert("WhiteAppleby Capital Wallet", "A new account was created", "Ok");
                    }
                    else
                        await DisplayAlert("WhiteAppleby Capital Wallet", "No account was created because you already have a registered account", "Ok");

                    txtAddress.Text = string.Format("Address: {0}", wallet.Address);
                    lblBalance.Text = string.Format("Balance: {0} ETH (at block {1})", (await wallet.FetchBalanceAsOfLastProcessedBlock()).WeiToEth(), wallet.LastProcessedBlock.Number);
                    lblUpdatedAt.Text = string.Format(
                        "\tBalance and transactions were last updated at {0}",
                        wallet.LastProcessedBlock.Timestamp.LocalDateTime);
                }
                catch (Exception chingadamadre)
                {
                    Console.WriteLine("Message: " + chingadamadre.Message);
                    Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                    Console.WriteLine("Source: " + chingadamadre.Source);
                }
            }
            else
            {
                await DisplayAlert("WA Capital Wallet", result.ErrorMessage, "Ok");
            }
        }

        private async Task SyncAccount()
        {
            try
            {
                //Console.WriteLine("Synchronizing wallet with the network... ");

                var maxKnownBlockNumber = await wallet.GetMaxKnownRemoteBlock();

                //log.Debug("Max known block {number}", maxKnownBlockNumber);
                //log.Debug("Latest processed block {number}", wallet.LastProcessedBlock.Number);

                var catchupDelta = maxKnownBlockNumber - wallet.LastProcessedBlock.Number;
                var catchupMin = wallet.LastProcessedBlock.Number;

                //log.Debug("Catching up by {delta} blocks", catchupDelta);

                for (var currentBlockNumber = wallet.LastProcessedBlock.Number;
                     currentBlockNumber <= maxKnownBlockNumber;
                     ++currentBlockNumber)
                {
                    var percentage = (double)(currentBlockNumber - catchupMin) / (double)catchupDelta;

                    /* log.Information( "Processing block {number} / {maxKnown} ({percentage:0.00}%)",
                        currentBlockNumber,
                        maxKnownBlockNumber,
                        percentage * 100.0);*/

                    await wallet.LoadAndSaveTransactionsForBlock(currentBlockNumber);
                }

                lblBalance.Text = string.Format("Balance: {0} ETH (at block {1})", (await wallet.FetchBalanceAsOfLastProcessedBlock()).WeiToEth(), wallet.LastProcessedBlock.Number);
                lblUpdatedAt.Text = string.Format(
                    "\tBalance and transactions were last updated at {0}",
                    wallet.LastProcessedBlock.Timestamp.LocalDateTime);
            }
            catch (Exception chingadamadre)
            {
                Console.WriteLine("Message: " + chingadamadre.Message);
                Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                Console.WriteLine("Source: " + chingadamadre.Source);
            }
        }

        private async Task SendTransaction()
        {
            try
            {
                //string recipient = "0xa8cfAd2E5c1263567b56030b34665244566ec173";
                string recipient = "0x1be90ba4cd2eed4077a0a0dc23e18965cd32e739";
                decimal amountInEth = 0.0001M;

                BigInteger gasPriceInGwei = new BigInteger(40);
                gasPriceInGwei = (BigInteger)25;

                BigInteger gasAllowance = new BigInteger(21000);

                BigInteger? nonceOverride = null;

                var maxFeeInWei = gasAllowance * gasPriceInGwei;
                var maxFeeInEth = maxFeeInWei.WeiToEth();

                var amountInWei = amountInEth.EthToWei();
                var maxExpenditureInWei = maxFeeInWei + amountInWei;
                var knownBalanceInWei = await wallet.FetchBalanceAsOfLastProcessedBlock();

                /*if (maxExpenditureInWei > knownBalanceInWei)
                {
                    await DisplayAlert("WA Capital Wallet", "SORRY! I can not send this transaction: This transaction may require more ETH than you have.", "Ok");
                    return;
                }*/

                var hash = await wallet.SendTransaction(
                        recipient,
                        amountInWei,
                        gasPriceInGwei.GweiToWei(),
                        gasAllowance,
                        nonceOverride);

            }
            catch (Exception chingadamadre)
            {
                Console.WriteLine("Message: " + chingadamadre.Message);
                Console.WriteLine("InnerException: " + chingadamadre.InnerException);
                Console.WriteLine("Source: " + chingadamadre.Source);
            }
        }
    }
}
