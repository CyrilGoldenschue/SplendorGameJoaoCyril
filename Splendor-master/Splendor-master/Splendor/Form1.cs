﻿/**
 * \file      frmAddVideoGames.cs
 * \author    C.Goldenschue & J.Alipio-Penedo
 * \version   1.0
 * \date      August 22. 2018
 * \brief     Form to play.
 *
 * \details   This form enables to choose coins or cards to get ressources (precious stones) and prestige points 
 * to add and to play with other players
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Splendor
{
    /// <summary>
    /// manages the form that enables to play with the Splendor
    /// </summary>
    public partial class frmSplendor : Form
    {
        
        //connection to the database
        private ConnectionDB conn = new ConnectionDB();
        //used to store the number of coins selected for the current round of game
        private int nbRubis;
        private int nbOnyx;
        private int nbEmeraude;
        private int nbDiamand;
        private int nbSaphir;
        private int nbTotal;
        private int nbRubis = 0;
        private int nbOnyx = 0;
        private int nbEmeraude = 0;
        private int nbDiamand = 0;
        private int nbSaphir = 0;
        private Stack<Card> listCardOne = new Stack<Card>();
        private Stack<Card> listCardTwo = new Stack<Card>();
        private Stack<Card> listCardThree = new Stack<Card>();
        private Stack<Card> listCardNoble = new Stack<Card>();
        private int nbCardRubis = 0;
        private int nbCardEmeraude = 0;
        private int nbCardOnyx = 0;
        private int nbCardSaphire = 0;
        private int nbCardDiamant = 0;
        private int[] NbPtPrestige;
        private char[] MyChar = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private int id = 0;
        private bool Reid = true;
        private int[,] Coin;
        private int[,] Ressource;
        private string NameControlAll = "";
        private FormAddPlayer form2;
        private int nbTotal = 0;
        private string[] NumCardAll = new string[3];
        private string NameCard;
        private Control DisableControler = new Control();
        private string[] Booked = new string[4];
        private int[] NbTour = new int[4];
        private int NbJeton = 0;
        private Random rnd = new Random();

        //id of the player that is playing
        private int currentPlayerId;
        //boolean to enable us to know if the user can click on a coin or a card
        private bool enableClicLabel;
        
        /// <summary>
        /// constructor
        /// </summary>
        public frmSplendor()
        {
            InitializeComponent();
        }
       
        /// <summary>
        /// loads the form and initialize data in it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void frmSplendor_Load(object sender, EventArgs e)
        {
            int NbPlayer = conn.GetCountPlayer();                   //get the number of players that are in the database

            lblGoldCoin.Text = "5";                                 //gives basic values
            lblPlayerGoldCoin.Text = "0";
            lblDiamandCoin.Text = "7";
            lblEmeraudeCoin.Text = "7";
            lblOnyxCoin.Text = "7";
            lblRubisCoin.Text = "7";
            lblSaphirCoin.Text = "7";

            Coin = new int[conn.NumberPlayer, 5];
            Ressource = new int[conn.NumberPlayer, 5];

            //load cards from the database
            listCardOne = conn.GetListCardAccordingToLevel(1);
            listCardTwo = conn.GetListCardAccordingToLevel(2);
            listCardThree = conn.GetListCardAccordingToLevel(3);
            listCardNoble = conn.GetListCardAccordingToLevel(4);

            Shuffle(listCardOne);                                   //Shuffle stack of card
            Shuffle(listCardTwo);
            Shuffle(listCardThree);
            Shuffle(listCardNoble);

            // Card level one
            txtLevel11.Text = listCardOne.Pop().ToString();         //Insert card in a textbox
            txtLevel12.Text = listCardOne.Pop().ToString();
            txtLevel13.Text = listCardOne.Pop().ToString();
            txtLevel14.Text = listCardOne.Pop().ToString();
            // Card level two
            txtLevel21.Text = listCardTwo.Pop().ToString();         //Insert card in a textbox
            txtLevel22.Text = listCardTwo.Pop().ToString();
            txtLevel23.Text = listCardTwo.Pop().ToString();
            txtLevel24.Text = listCardTwo.Pop().ToString();
            // Card level three
            txtLevel31.Text = listCardThree.Pop().ToString();       //Insert card in a textbox
            txtLevel32.Text = listCardThree.Pop().ToString();             
            txtLevel33.Text = listCardThree.Pop().ToString();
            txtLevel34.Text = listCardThree.Pop().ToString();
            // Card level Noble
            txtNoble1.Text = listCardNoble.Pop().ToString();        //Insert card in a textbox
            txtNoble2.Text = listCardNoble.Pop().ToString();
            txtNoble3.Text = listCardNoble.Pop().ToString();
            txtNoble4.Text = listCardNoble.Pop().ToString();

            this.Width = 680;                                       //Change the resolution of frmSplendor
            this.Height = 540;

            enableClicLabel = false;

            lblChoiceDiamant.Visible = false;                       //erases the labels of choice
            lblChoiceOnyx.Visible = false;
            lblChoiceRubis.Visible = false;
            lblChoiceSaphire.Visible = false;
            lblChoiceEmeraude.Visible = false;
            cmdValidateChoice.Visible = false;                      //erases the buttons of validate choice
            cmdNextPlayer.Visible = false;                          //erases the buttons of next player

            //we wire the click on all cards to the same event

            // Click card level one
            txtLevel11.Click += ClickOnCard;
            txtLevel12.Click += ClickOnCard;
            txtLevel13.Click += ClickOnCard;
            txtLevel14.Click += ClickOnCard;

            // Click card level two
            txtLevel21.Click += ClickOnCard;                        //When we click it buys the card
            txtLevel22.Click += ClickOnCard;
            txtLevel23.Click += ClickOnCard;
            txtLevel24.Click += ClickOnCard;

            // Click card level three
            txtLevel31.Click += ClickOnCard;                         //When we click it buys the card
            txtLevel32.Click += ClickOnCard;
            txtLevel33.Click += ClickOnCard;
            txtLevel34.Click += ClickOnCard;

            // Click card level Noble
            txtNoble1.Click += ClickOnCard;                        //When we click it buys the card
            txtNoble2.Click += ClickOnCard;
            txtNoble3.Click += ClickOnCard;
            txtNoble4.Click += ClickOnCard;

            // Click card booked
            txtPlayerBookedCard.Click += ClickOnBooked;             //When we click it buys the booked card
        }

        /// <summary>
        /// mix of stacks of cards
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        private void Shuffle<T>(Stack<T> stack)
        {
            var values = stack.ToArray();                           //Stock value of stack
            stack.Clear();                                          //Clear value of stack
            foreach (var value in values.OrderBy(x => rnd.Next()))  //swap the space between two cards
                stack.Push(value);                                  //Send the stored card instead of another

        }

        /// <summary>
        /// Purchase card booked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickOnBooked(object sender, EventArgs e)
        {
            TextBox CardBooked = (TextBox)sender;
            
            //Nombre de carte de ressource
            int NbCardRubis = Convert.ToInt16(txtPlayerRubisCard.Text);
            int NbCardSaphire = Convert.ToInt16(txtPlayerSaphireCard.Text);
            int NbCardOnyx = Convert.ToInt16(txtPlayerOnyxCard.Text);
            int NbCardEmeraude = Convert.ToInt16(txtPlayerEmeraudeCard.Text);
            int NbCardDiamant = Convert.ToInt16(txtPlayerDiamantCard.Text);
            //Nombre de ressources en stock
            int NbRubisPoss = Convert.ToInt16(lblPlayerRubisCoin.Text);
            int NbSaphirePoss = Convert.ToInt16(lblPlayerSaphireCoin.Text);
            int NbEmeraudePoss = Convert.ToInt16(lblPlayerEmeraudeCoin.Text);
            int NbOnyxPoss = Convert.ToInt16(lblPlayerOnyxCoin.Text);
            int NbDiamantPoss = Convert.ToInt16(lblPlayerDiamantCoin.Text);
            int NbGoldPoss = Convert.ToInt16(lblPlayerGoldCoin.Text);
            //Nombre de ressources restantes
            int BanqueRubis = Convert.ToInt16(lblRubisCoin.Text);
            int BanqueSaphire = Convert.ToInt16(lblSaphirCoin.Text);
            int BanqueEmeraude = Convert.ToInt16(lblEmeraudeCoin.Text);
            int BanqueOnyx = Convert.ToInt16(lblOnyxCoin.Text);
            int BanqueDiamant = Convert.ToInt16(lblDiamandCoin.Text);
            int BanqueGold = Convert.ToInt16(lblGoldCoin.Text);

            int NbRessource = 2;
            int NbRessourceMiss = 0;
            string[] TabComparatif;
            TabComparatif = CardBooked.Lines;
            int NbLines = CardBooked.Lines.Count() - 1;
            bool Achat = true;

            while (NbRessource != NbLines)
            {
                if ((TabComparatif[NbRessource].Substring(4)).StartsWith("Rubis"))
                {
                    if (NbRubisPoss + NbCardRubis + NbGoldPoss >= Convert.ToInt16(TabComparatif[NbRessource].Substring(10)))
                    {
                        if (NbRubisPoss + NbCardRubis >= Convert.ToInt16(TabComparatif[NbRessource].Substring(10)))
                        {
                            NbRubisPoss = NbRubisPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(10)) + NbCardRubis;
                            BanqueRubis = BanqueRubis + Convert.ToInt16(TabComparatif[NbRessource].Substring(10)) - NbCardRubis;
                        }
                        else
                        {
                            NbRubisPoss = NbRubisPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(10)) + NbCardRubis + NbGoldPoss;
                            BanqueGold = BanqueGold + 1;
                            NbGoldPoss = NbGoldPoss - 1;

                        }
                        lblRubisCoin.Text = BanqueRubis.ToString();
                        lblGoldCoin.Text = BanqueGold.ToString();

                    }
                    else
                    {
                        int NbRubisMiss = Convert.ToInt16(TabComparatif[NbRessource].Substring(10)) - NbRubisPoss;
                        if (NbRubisMiss == 1)
                        {
                            NbRessourceMiss++;
                        }
                    }
                }
                else if ((TabComparatif[NbRessource].Substring(4)).StartsWith("Saphire"))
                {

                    if (NbSaphirePoss + NbCardSaphire + NbGoldPoss >= Convert.ToInt16(TabComparatif[NbRessource].Substring(12)))
                    {
                        if (NbSaphirePoss + NbCardSaphire >= Convert.ToInt16(TabComparatif[NbRessource].Substring(12)))
                        {
                            NbSaphirePoss = NbSaphirePoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) + NbCardSaphire;
                            BanqueSaphire = BanqueSaphire + Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) - NbCardSaphire;
                        }
                        else
                        {
                            NbSaphirePoss = NbSaphirePoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) + NbCardSaphire + NbGoldPoss;
                            BanqueGold = BanqueGold + 1;
                            NbGoldPoss = NbGoldPoss - 1;

                        }

                        lblSaphirCoin.Text = BanqueSaphire.ToString();
                        lblGoldCoin.Text = BanqueGold.ToString();

                    }
                    else
                    {
                        int NbSaphireMiss = Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) - NbSaphirePoss;
                        if (NbSaphireMiss == 1)
                        {
                            NbRessourceMiss++;
                        }
                    }
                }
                else if ((TabComparatif[NbRessource].Substring(4)).StartsWith("Emeraude"))
                {
                    if (NbEmeraudePoss + NbCardEmeraude + NbGoldPoss >= Convert.ToInt16(TabComparatif[NbRessource].Substring(13)))
                    {
                        if (NbEmeraudePoss + NbCardEmeraude >= Convert.ToInt16(TabComparatif[NbRessource].Substring(13)))
                        {
                            NbEmeraudePoss = NbEmeraudePoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(13)) + NbCardEmeraude;
                            BanqueEmeraude = BanqueEmeraude + Convert.ToInt16(TabComparatif[NbRessource].Substring(13)) - NbCardEmeraude;
                        }
                        else
                        {
                            NbEmeraudePoss = NbEmeraudePoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(13)) + NbCardEmeraude + NbGoldPoss;
                            BanqueGold = BanqueGold + 1;
                            NbGoldPoss = NbGoldPoss - 1;

                        }

                        lblEmeraudeCoin.Text = BanqueEmeraude.ToString();
                        lblGoldCoin.Text = BanqueGold.ToString();

                    }
                    else
                    {
                        int NbEmeraudeMiss = Convert.ToInt16(TabComparatif[NbRessource].Substring(13)) - NbEmeraudePoss;
                        if (NbEmeraudeMiss == 1)
                        {
                            NbRessourceMiss++;
                        }
                    }
                }
                else if ((TabComparatif[NbRessource].Substring(4)).StartsWith("Onyx"))
                {
                    if (NbOnyxPoss + NbCardOnyx + NbGoldPoss >= Convert.ToInt16(TabComparatif[NbRessource].Substring(9)))
                    {
                        if (NbRubisPoss + NbCardOnyx >= Convert.ToInt16(TabComparatif[NbRessource].Substring(9)))
                        {
                            NbOnyxPoss = NbOnyxPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(9)) + NbCardOnyx;
                            BanqueOnyx = BanqueOnyx + Convert.ToInt16(TabComparatif[NbRessource].Substring(9)) - NbCardOnyx;
                        }
                        else
                        {
                            NbOnyxPoss = NbOnyxPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(9)) + NbCardOnyx + NbGoldPoss;
                            BanqueGold = BanqueGold + 1;
                            NbGoldPoss = NbGoldPoss - 1;

                        }

                        lblOnyxCoin.Text = BanqueOnyx.ToString();
                        lblGoldCoin.Text = BanqueGold.ToString();

                    }
                    else
                    {
                        int NbOnyxMiss = Convert.ToInt16(TabComparatif[NbRessource].Substring(9)) - NbOnyxPoss;
                        if (NbOnyxMiss == 1)
                        {
                            NbRessourceMiss++;
                        }
                    }
                }
                else if ((TabComparatif[NbRessource].Substring(4)).StartsWith("Diamant"))
                {
                    if (NbDiamantPoss + NbCardDiamant + NbGoldPoss >= Convert.ToInt16(TabComparatif[NbRessource].Substring(12)))
                    {
                        if (NbDiamantPoss + NbCardOnyx >= Convert.ToInt16(TabComparatif[NbRessource].Substring(12)))
                        {
                            NbDiamantPoss = NbDiamantPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) + NbCardDiamant;
                            BanqueDiamant = BanqueDiamant + Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) - NbCardDiamant;
                        }
                        else
                        {
                            NbDiamantPoss = NbDiamantPoss - Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) + NbCardDiamant + NbGoldPoss;
                            BanqueGold = BanqueGold + 1;
                            NbGoldPoss = NbGoldPoss - 1;
                        }

                        lblDiamandCoin.Text = BanqueDiamant.ToString();
                        lblGoldCoin.Text = BanqueGold.ToString();
                    }
                    else
                    {
                        int NbDiamantMiss = Convert.ToInt16(TabComparatif[NbRessource].Substring(12)) - NbDiamantPoss;
                        if (NbDiamantMiss == 1)
                        {
                            NbRessourceMiss++;
                        }
                    }
                }

                if (NbRessource + 1 != NbLines)
                {
                    NbRessource++;
                }
                else
                {
                    break;
                }
            }
            if(NbRessourceMiss > 1)
            {
                Achat = false;
            }
            if (Achat)
            {
                CardBooked.Clear();

                lblPlayerRubisCoin.Text = NbRubisPoss.ToString();
                lblPlayerSaphireCoin.Text = NbSaphirePoss.ToString();
                lblPlayerOnyxCoin.Text = NbOnyxPoss.ToString();
                lblPlayerEmeraudeCoin.Text = NbEmeraudePoss.ToString();
                lblPlayerDiamantCoin.Text = NbDiamantPoss.ToString();
                lblPlayerGoldCoin.Text = NbGoldPoss.ToString();
                Booked[id] = null;
                
                if (TabComparatif[0].StartsWith("Rubis"))
                {
                    nbCardRubis++;
                    txtPlayerRubisCard.Text = nbCardRubis.ToString();

                    if (TabComparatif[0].Substring(7) != " ")
                    {
                        if (TabComparatif[0].Substring(9) != "\t")
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TabComparatif[0].Substring(7));
                        }
                    }
                }
                else if (TabComparatif[0].StartsWith("Emeraude"))
                {
                    NbCardEmeraude++;
                    txtPlayerEmeraudeCard.Text = NbCardEmeraude.ToString();
                    if (TabComparatif[0].Substring(10) != " ")
                    {
                        if (TabComparatif[0].Substring(12) != "\t")
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TabComparatif[0].Substring(8));
                        }

                    }
                }
                else if (TabComparatif[0].StartsWith("Onyx"))
                {
                    nbCardOnyx++;
                    txtPlayerOnyxCard.Text = nbCardOnyx.ToString();
                    if (TabComparatif[0].Substring(6) != " ")
                    {
                        if (TabComparatif[0].Substring(8) != "\t")
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TabComparatif[0].Substring(6));
                        }
                    }
                }
                else if (TabComparatif[0].StartsWith("Saphire"))
                {
                    nbCardSaphire++;
                    txtPlayerSaphireCard.Text = nbCardSaphire.ToString();
                    if (TabComparatif[0].Substring(9) != " ")
                    {
                        if (TabComparatif[0].Substring(11) != "\t")
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TabComparatif[0].Substring(9));
                        }
                    }
                }
                else if (TabComparatif[0].StartsWith("Diamant"))
                {
                    nbCardDiamant++;
                    txtPlayerDiamantCard.Text = nbCardDiamant.ToString();
                    if (TabComparatif[0].Substring(9) != " ")
                    {
                        if (TabComparatif[0].Substring(11) != "\t")
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TabComparatif[0].Substring(9));
                        }
                    }
                }
                
                lblNbPtPrestige.Text = "Nb pt prestige : " + NbPtPrestige[id];
                cmdNextPlayer.Visible = true;
                cmdNextPlayer.Enabled = true;
                cmdValidateChoice.Enabled = false;
                // disable card Level one
                txtLevel11.Enabled = false;
                txtLevel12.Enabled = false;
                txtLevel13.Enabled = false;
                txtLevel14.Enabled = false;
                // disable card Level two
                txtLevel21.Enabled = false;
                txtLevel22.Enabled = false;
                txtLevel23.Enabled = false;
                txtLevel24.Enabled = false;
                // disable card Level three
                txtLevel31.Enabled = false;
                txtLevel32.Enabled = false;
                txtLevel33.Enabled = false;
                txtLevel34.Enabled = false;
                // disable lblCoin
                lblRubisCoin.Enabled = false;
                lblSaphirCoin.Enabled = false;
                lblEmeraudeCoin.Enabled = false;
                lblOnyxCoin.Enabled = false;
                lblDiamandCoin.Enabled = false;

            }

            txtPlayerBookedCard.Enabled = false;
        }

        private void ClickOnCard(object sender, EventArgs e)
        {
            //We get the value on the card and we split it to get all the values we need (number of prestige points and ressource)
            //Enable the button "Validate"
            //TO DO
            if (enableClicLabel)
            {

                TextBox cardOnBoard = (TextBox)sender;
                NameControlAll = cardOnBoard.Name;
                NameCard = cardOnBoard.Name.ToString();

                string[] TableauComparatif; //= new string[cardOnBoard.Lines.Count()];
  
                cardOnBoard.Visible = true;
                string test = cardOnBoard.Text;
               // string[6] TableauComparatif = new string[6];
                TableauComparatif = cardOnBoard.Lines;

                int NbLines = cardOnBoard.Lines.Count()-1;
                //Nombre de carte de ressource
                int NbCardRubis = Convert.ToInt16(txtPlayerRubisCard.Text);
                int NbCardSaphire = Convert.ToInt16(txtPlayerSaphireCard.Text);
                int NbCardOnyx = Convert.ToInt16(txtPlayerOnyxCard.Text);
                int NbCardEmeraude = Convert.ToInt16(txtPlayerEmeraudeCard.Text);
                int NbCardDiamant = Convert.ToInt16(txtPlayerDiamantCard.Text);
                //Nombre de ressources en stock
                int NbRubisPoss = Convert.ToInt16(lblPlayerRubisCoin.Text);
                int NbSaphirePoss = Convert.ToInt16(lblPlayerSaphireCoin.Text);
                int NbEmeraudePoss = Convert.ToInt16(lblPlayerEmeraudeCoin.Text);
                int NbOnyxPoss = Convert.ToInt16(lblPlayerOnyxCoin.Text);
                int NbDiamantPoss = Convert.ToInt16(lblPlayerDiamantCoin.Text);
                int NbGoldPoss = Convert.ToInt16(lblPlayerGoldCoin.Text);
                //Nombre de ressources restantes
                int BanqueRubis = Convert.ToInt16(lblRubisCoin.Text);
                int BanqueSaphire = Convert.ToInt16(lblSaphirCoin.Text);
                int BanqueEmeraude = Convert.ToInt16(lblEmeraudeCoin.Text);
                int BanqueOnyx = Convert.ToInt16(lblOnyxCoin.Text);
                int BanqueDiamant = Convert.ToInt16(lblDiamandCoin.Text);
                int BanqueGold = Convert.ToInt16(lblGoldCoin.Text);
                //Nombre de cartes restantes
                int NbCardResidualOne = listCardOne.Count();
                int NbCardResidualTwo = listCardTwo.Count();
                int NbCardResidualThree = listCardThree.Count();
                int NbCardResidualNoble = listCardNoble.Count();


                string SorteRessourceMiss = "Vous n'avez pas assez de ";
                int NbRessource = 2;
                int NbRessourceMiss = 0;
                //Boolean pour savoir si le joueur achète la carte ou non
                bool Achat = true;

                if (NameCard.Substring(1, 8) != "txtNoble")
                {
                    while (NbRessource != NbLines)
                    {
                        if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Rubis"))
                        {
                            if (NbRubisPoss + NbCardRubis >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(10)))
                            {
                                //MessageBox.Show("C'est du Rubis");
                                NbRubisPoss = NbRubisPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(10)) + NbCardRubis;
                                lblRubisCoin.Text = BanqueRubis.ToString();
                            }
                            else
                            {
                                int NbRubisMiss = NbRubisPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(10));
                                SorteRessourceMiss += "Rubis ";
                                Achat = false;
                                if(NbRubisMiss == 1)
                                {
                                    NbRessourceMiss++;
                                }
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Saphire"))
                        {

                            if (NbSaphirePoss + NbCardSaphire >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)))
                            {
                                //MessageBox.Show("C'est du Saphire");
                                NbSaphirePoss = NbSaphirePoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)) + NbCardSaphire;
                                lblSaphirCoin.Text = BanqueSaphire.ToString();
                            }
                            else
                            {
                                int NbSaphireMiss = NbSaphirePoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(12));
                                SorteRessourceMiss += "Saphire ";
                                Achat = false;
                                if (NbSaphireMiss == 1)
                                {
                                    NbRessourceMiss++;
                                }
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Emeraude"))
                        {
                            if (NbEmeraudePoss + NbCardEmeraude >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(13)))
                            {
                                //MessageBox.Show("C'est de l'Emeraude");
                                NbEmeraudePoss = NbEmeraudePoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(13)) + NbCardEmeraude;
                                lblEmeraudeCoin.Text = BanqueEmeraude.ToString();
                            }
                            else
                            {
                                int NbEmeraudeMiss = NbEmeraudePoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(13));
                                    SorteRessourceMiss += "Emeraude ";
                                Achat = false;
                                if (NbEmeraudeMiss == 1)
                                {
                                    NbRessourceMiss++;
                                }
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Onyx"))
                        {
                            if (NbOnyxPoss + NbCardOnyx >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(9)))
                            {
                                //MessageBox.Show("C'est de l'Onyx");
                                NbOnyxPoss = NbOnyxPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(9)) + NbCardOnyx;
                                lblOnyxCoin.Text = BanqueOnyx.ToString();
                            }
                            else
                            {
                                int NbOnyxMiss = NbOnyxPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(9));
                                SorteRessourceMiss += "Onyx ";
                                Achat = false;
                                if (NbOnyxMiss == 1)
                                {
                                    NbRessourceMiss++;
                                }
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Diamant"))
                        {
                            if (NbDiamantPoss + NbCardDiamant >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)))
                            {
                                //MessageBox.Show("C'est du Diamant");
                                NbDiamantPoss = NbDiamantPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)) + NbCardDiamant;
                                lblDiamandCoin.Text = BanqueDiamant.ToString();
                            }
                            else
                            {
                                int NbDiamantMiss = NbDiamantPoss - Convert.ToInt16(TableauComparatif[NbRessource].Substring(12));
                                SorteRessourceMiss += "Diamant ";
                                Achat = false;
                                if (NbDiamantMiss <= 1)
                                {
                                    NbRessourceMiss++;
                                    
                                }
                            }
                        }
                        
                        if (NbRessource + 1 != NbLines)
                        {
                            NbRessource++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (NbRessourceMiss == 1)
                    {
                        if(MessageBox.Show("il vous manque une ressource. Voulez-vous réserver la carte ?", "Message de confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            lblGoldCoin.Text = "" + (BanqueGold - 1) + "";
                            lblPlayerGoldCoin.Text = "" + (NbGoldPoss + 1) + "";
                            Booked[id] = cardOnBoard.Text;
                            NbTour[id] = 1;
                            txtPlayerBookedCard.Text = cardOnBoard.Text;
                            cardOnBoard.Clear();
                            if (cardOnBoard.Name == "txtLevel11" | cardOnBoard.Name == "txtLevel12" | cardOnBoard.Name == "txtLevel13" | cardOnBoard.Name == "txtLevel14")
                            {
                                cardOnBoard.Text = listCardOne.Pop().ToString();
                                NbCardResidualOne--;
                            }
                            else if (cardOnBoard.Name == "txtLevel21" | cardOnBoard.Name == "txtLevel22" | cardOnBoard.Name == "txtLevel23" | cardOnBoard.Name == "txtLevel24")
                            {
                                cardOnBoard.Text = listCardTwo.Pop().ToString();
                                NbCardResidualTwo--;
                            }
                            else if (cardOnBoard.Name == "txtLevel31" | cardOnBoard.Name == "txtLevel32" | cardOnBoard.Name == "txtLevel33" | cardOnBoard.Name == "txtLevel34")
                            {
                                cardOnBoard.Text = listCardThree.Pop().ToString();
                                NbCardResidualThree--;
                            }
                            cmdNextPlayer.Visible = true;
                            cmdNextPlayer.Enabled = true;
                            cmdValidateChoice.Enabled = false;
                            // disable card Level one
                            txtLevel11.Enabled = false;
                            txtLevel12.Enabled = false;
                            txtLevel13.Enabled = false;
                            txtLevel14.Enabled = false;
                            // disable card Level two
                            txtLevel21.Enabled = false;
                            txtLevel22.Enabled = false;
                            txtLevel23.Enabled = false;
                            txtLevel24.Enabled = false;
                            // disable card Level three
                            txtLevel31.Enabled = false;
                            txtLevel32.Enabled = false;
                            txtLevel33.Enabled = false;
                            txtLevel34.Enabled = false;
                            txtPlayerBookedCard.Enabled = false;
                            // disable lblCoin
                            lblRubisCoin.Enabled = false;
                            lblSaphirCoin.Enabled = false;
                            lblEmeraudeCoin.Enabled = false;
                            lblOnyxCoin.Enabled = false;
                            lblDiamandCoin.Enabled = false;
                        }
                    }
                    if (Achat)
                    {
                        cardOnBoard.Clear();
                        //NumCardAll[Convert.ToInt16(NameCard.Substring(8)) - 1] = NameCard;


                        if (cardOnBoard.Name == "txtLevel11" | cardOnBoard.Name == "txtLevel12" | cardOnBoard.Name == "txtLevel13" | cardOnBoard.Name == "txtLevel14")
                        {
                            cardOnBoard.Text = listCardOne.Pop().ToString();
                            NbCardResidualOne--;
                        }
                        else if (cardOnBoard.Name == "txtLevel21" | cardOnBoard.Name == "txtLevel22" | cardOnBoard.Name == "txtLevel23" | cardOnBoard.Name == "txtLevel24")
                        {
                            cardOnBoard.Text = listCardTwo.Pop().ToString();
                            NbCardResidualTwo--;
                        }
                        else if (cardOnBoard.Name == "txtLevel31" | cardOnBoard.Name == "txtLevel32" | cardOnBoard.Name == "txtLevel33" | cardOnBoard.Name == "txtLevel34")
                        {
                            cardOnBoard.Text = listCardThree.Pop().ToString();
                            NbCardResidualThree--;
                        }

                        lblPlayerRubisCoin.Text = NbRubisPoss.ToString();
                        lblPlayerSaphireCoin.Text = NbSaphirePoss.ToString();
                        lblPlayerOnyxCoin.Text = NbOnyxPoss.ToString();
                        lblPlayerEmeraudeCoin.Text = NbEmeraudePoss.ToString();
                        lblPlayerDiamantCoin.Text = NbDiamantPoss.ToString();


                        if (TableauComparatif[NbRessource].StartsWith("Rubis"))
                        {
                            nbCardRubis++;
                            BanqueRubis = BanqueRubis + Convert.ToInt16(TableauComparatif[NbRessource].Substring(10));
                            txtPlayerRubisCard.Text = nbCardRubis.ToString();

                            if (TableauComparatif[0].Substring(7) != " ")
                            {
                                if (TableauComparatif[0].Substring(9) != "\t")
                                {
                                    NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(7));
                                }
                            }
                        }
                        else if (TableauComparatif[NbRessource].StartsWith("Emeraude"))
                        {
                            nbCardEmeraude++;
                            BanqueEmeraude = BanqueEmeraude + Convert.ToInt16(TableauComparatif[NbRessource].Substring(13));
                            txtPlayerEmeraudeCard.Text = nbCardEmeraude.ToString();
                            if (TableauComparatif[0].Substring(10) != " ")
                            {
                                if (TableauComparatif[0].Substring(12) != "\t")
                                {
                                    NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(8));
                                }

                            }
                        }
                        else if (TableauComparatif[NbRessource].StartsWith("Onyx"))
                        {
                            nbCardOnyx++;
                            txtPlayerOnyxCard.Text = nbCardOnyx.ToString();
                            BanqueOnyx = BanqueOnyx + Convert.ToInt16(TableauComparatif[NbRessource].Substring(9));
                            if (TableauComparatif[0].Substring(6) != " ")
                            {
                                if (TableauComparatif[0].Substring(8) != "\t")
                                {
                                    NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(6));
                                }
                            }
                        }
                        else if (TableauComparatif[NbRessource].StartsWith("Saphire"))
                        {
                            nbCardSaphire++;
                            BanqueSaphire = BanqueSaphire + Convert.ToInt16(TableauComparatif[NbRessource].Substring(12));
                            txtPlayerSaphireCard.Text = nbCardSaphire.ToString();
                            if (TableauComparatif[0].Substring(9) != " ")
                            {
                                if (TableauComparatif[0].Substring(11) != "\t")
                                {
                                    NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(9));
                                }
                            }
                        }
                        else if (TableauComparatif[NbRessource].StartsWith("Diamant"))
                        {
                            nbCardDiamant++;
                            BanqueDiamant = BanqueDiamant + Convert.ToInt16(TableauComparatif[NbRessource].Substring(12));
                            txtPlayerDiamantCard.Text = nbCardDiamant.ToString();
                            if (TableauComparatif[0].Substring(9) != " ")
                            {
                                if (TableauComparatif[0].Substring(11) != "\t")
                                {
                                    NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(9));
                                }
                            }
                        }
                        else if (TableauComparatif[0].StartsWith(" "))
                        {
                            NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(2));
                        }

                        lblNbPtPrestige.Text = "Nb pt prestige : " + NbPtPrestige[id];
                        cmdNextPlayer.Visible = true;
                        cmdNextPlayer.Enabled = true;
                        cmdValidateChoice.Enabled = false;
                        // disable card Level one
                        txtLevel11.Enabled = false;
                        txtLevel12.Enabled = false;
                        txtLevel13.Enabled = false;
                        txtLevel14.Enabled = false;
                        // disable card Level two
                        txtLevel21.Enabled = false;
                        txtLevel22.Enabled = false;
                        txtLevel23.Enabled = false;
                        txtLevel24.Enabled = false;
                        // disable card Level three
                        txtLevel31.Enabled = false;
                        txtLevel32.Enabled = false;
                        txtLevel33.Enabled = false;
                        txtLevel34.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show(SorteRessourceMiss);
                    }
                }
                else
                {
                    //Purchase of noble card
                    while (NbRessource != NbLines)
                    {
                        if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Rubis"))
                        {
                            if (NbCardRubis >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(10)))
                            {
                                //MessageBox.Show("C'est du Rubis");
                            }
                            else
                            {
                                SorteRessourceMiss += "Rubis ";
                                Achat = false;
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Saphire"))
                        {

                            if (NbCardSaphire >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)))
                            {
                                //MessageBox.Show("C'est du Saphire");
                            }
                            else
                            {
                                SorteRessourceMiss += "Saphire ";
                                Achat = false;
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Emeraude"))
                        {
                            if (NbCardEmeraude >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(13)))
                            {
                                //MessageBox.Show("C'est de l'Emeraude");
                            }
                            else
                            {
                                SorteRessourceMiss += "Emeraude ";
                                Achat = false;
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Onyx"))
                        {
                            if (NbCardOnyx >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(9)))
                            {
                                //MessageBox.Show("C'est de l'Onyx");
                            }
                            else
                            {
                                SorteRessourceMiss += "Onyx ";
                                Achat = false;
                            }
                        }
                        else if ((TableauComparatif[NbRessource].Substring(4)).StartsWith("Diamant"))
                        {
                            if (NbCardDiamant >= Convert.ToInt16(TableauComparatif[NbRessource].Substring(12)))
                            {
                                //MessageBox.Show("C'est du Diamant");
                            }
                            else
                            {
                                SorteRessourceMiss += "Diamant ";
                                Achat = false;
                            }
                        }

                        if (NbRessource + 1 != NbLines)
                        {
                            NbRessource++;
                        }
                        else
                        {
                            break;
                        }
                        if (Achat)
                        {
                            cardOnBoard.Clear();

                            if (TableauComparatif[NbRessource].StartsWith("Rubis"))
                            {
                                nbCardRubis++;
                                txtPlayerRubisCard.Text = nbCardRubis.ToString();

                                if (TableauComparatif[0].Substring(7) != " ")
                                {
                                    if (TableauComparatif[0].Substring(9) != "\t")
                                    {
                                        NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(7));
                                    }
                                }
                            }
                            else if (TableauComparatif[NbRessource].StartsWith("Emeraude"))
                            {
                                nbCardEmeraude++;
                                txtPlayerEmeraudeCard.Text = nbCardEmeraude.ToString();
                                if (TableauComparatif[0].Substring(10) != " ")
                                {
                                    if (TableauComparatif[0].Substring(12) != "\t")
                                    {
                                        NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(8));
                                    }

                                }
                            }
                            else if (TableauComparatif[NbRessource].StartsWith("Onyx"))
                            {
                                nbCardOnyx++;
                                txtPlayerOnyxCard.Text = nbCardOnyx.ToString();
                                if (TableauComparatif[0].Substring(6) != " ")
                                {
                                    if (TableauComparatif[0].Substring(8) != "\t")
                                    {
                                        NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(6));
                                    }
                                }
                            }
                            else if (TableauComparatif[NbRessource].StartsWith("Saphire"))
                            {
                                nbCardSaphire++;
                                txtPlayerSaphireCard.Text = nbCardSaphire.ToString();
                                if (TableauComparatif[0].Substring(9) != " ")
                                {
                                    if (TableauComparatif[0].Substring(11) != "\t")
                                    {
                                        NbPtPrestige[id] += Convert.ToInt16(TableauComparatif[0].Substring(9));
                                    }
                                }
                            }
                            else if (TableauComparatif[NbRessource].StartsWith("Diamant"))
                            {
                                nbCardDiamant++;
                                txtPlayerDiamantCard.Text = nbCardDiamant.ToString();
                                if (TableauComparatif[0].Substring(9) != " ")
                                {
                                    if (TableauComparatif[0].Substring(11) != "\t")
                                    {
                                        NbPtPrestige[id] = Convert.ToInt16(TableauComparatif[0].Substring(9));
                                    }
                                }
                            }
                            else if (TableauComparatif[0].StartsWith(" "))
                            {
                                NbPtPrestige[id] = Convert.ToInt16(TableauComparatif[0].Substring(2));
                            }

                            lblNbPtPrestige.Text = "Nb pt prestige : " + NbPtPrestige[id];
                            cmdNextPlayer.Visible = true;
                            cmdNextPlayer.Enabled = true;
                            cmdValidateChoice.Enabled = false;
                            // disable card Level one
                            txtLevel11.Enabled = false;
                            txtLevel12.Enabled = false;
                            txtLevel13.Enabled = false;
                            txtLevel14.Enabled = false;
                            // disable card Level two
                            txtLevel21.Enabled = false;
                            txtLevel22.Enabled = false;
                            txtLevel23.Enabled = false;
                            txtLevel24.Enabled = false;
                            // disable card Level three
                            txtLevel31.Enabled = false;
                            txtLevel32.Enabled = false;
                            txtLevel33.Enabled = false;
                            txtLevel34.Enabled = false;
                            // disable card Noble
                            txtNoble1.Enabled = false;
                            txtNoble2.Enabled = false;
                            txtNoble3.Enabled = false;
                            //txtNoble4.Enabled = false;
                            txtNoble4.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show(SorteRessourceMiss);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// click on the play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdPlay_Click(object sender, EventArgs e)
        {
            int NbJeton = 0;

            int NbPlayer = conn.GetCountPlayer();

            switch (NbPlayer)
            {
                case 2:
                    NbJeton = 4;
                    break;
                case 3:
                    NbJeton = 5;
                    break;

                case 4:
                    NbJeton = 7;
                    break;
            }

            lblDiamandCoin.Text = NbJeton.ToString();
            lblEmeraudeCoin.Text = NbJeton.ToString();
            lblOnyxCoin.Text = NbJeton.ToString();
            lblRubisCoin.Text = NbJeton.ToString();
            lblSaphirCoin.Text = NbJeton.ToString();

            this.Width = 680;
            this.Height = 780;

            cmdInsertPlayer.Enabled = false;

            conn.NumberPlayer = conn.GetCountPlayer();

            Coin = new int[conn.NumberPlayer, 6];
            Coin[id, 0] = 0;
            Coin[id, 1] = 0;
            Coin[id, 2] = 0;
            Coin[id, 3] = 0;
            Coin[id, 4] = 0;
            Coin[id, 5] = 0;

            NbPtPrestige = new int[conn.GetCountPlayer()];
            NbPtPrestige[0] = 0;
            NbPtPrestige[1] = 0;
            if (conn.GetCountPlayer()-1 > 1)
            {
                NbPtPrestige[2] = 0;
                if (conn.GetCountPlayer()-1 > 2)
                {
                    NbPtPrestige[3] = 0;
                }
            }

            Ressource = new int[conn.NumberPlayer, 5];
            Ressource[id, 0] = 0;
            Ressource[id, 1] = 0;
            Ressource[id, 2] = 0;
            Ressource[id, 3] = 0;
            Ressource[id, 4] = 0;
            //Ressource[id, 5] = 0;

            LoadPlayer(id);

        }


        /// <summary>
        /// load data about the current player
        /// </summary>
        /// <param name="id">identifier of the player</param>
        private void LoadPlayer(int id) {
            txtPlayerBookedCard.Clear();
            currentPlayerId = id;
            enableClicLabel = true;
            if (conn.GetPlayerName(id) == "")
            {
                currentPlayerId++;
            }
            string name = conn.GetPlayerName(currentPlayerId);

            //no coins or card selected yet, labels are empty
            lblChoiceDiamant.Text = "";
            lblChoiceOnyx.Text = "";
            lblChoiceRubis.Text = "";
            lblChoiceSaphire.Text = "";
            lblChoiceEmeraude.Text = "";

            lblChoiceCard.Text = "";

            //no coins selected
            nbDiamand = 0;
            nbOnyx = 0;
            nbRubis = 0;
            nbSaphir = 0;
            nbEmeraude = 0;

            //Initialisation du joueur
            Player player = new Player();
            player.Name = name;
            player.Id = id;
            player.Ressources = new int[] { Ressource[id, 0], Ressource[id, 1], Ressource[id, 2], Ressource[id, 3], Ressource[id, 4]};
            player.Coins = new int[] { Coin[id, 0], Coin[id, 1], Coin[id, 2], Coin[id, 3], Coin[id, 4], Coin[id, 5] };

            if (Booked[id] != null)
            {
                //enable card booked
                txtPlayerBookedCard.Enabled = true;
                txtPlayerBookedCard.Text = Booked[id];
            }

            lblPlayerRubisCoin.Text = player.Coins[0].ToString();
            lblPlayerSaphireCoin.Text = player.Coins[1].ToString();
            lblPlayerOnyxCoin.Text = player.Coins[2].ToString();
            lblPlayerEmeraudeCoin.Text = player.Coins[3].ToString();
            lblPlayerDiamantCoin.Text = player.Coins[4].ToString();
            //Gold Coin
            //lblPlayerGoldCoin.Text = player.Coins[5].ToString();
            lblNbPtPrestige.Text = NbPtPrestige[id].ToString();

            txtPlayerRubisCard.Text = player.Ressources[0].ToString();
            txtPlayerSaphireCard.Text = player.Ressources[1].ToString();
            txtPlayerOnyxCard.Text = player.Ressources[2].ToString();
            txtPlayerEmeraudeCard.Text = player.Ressources[3].ToString();
            txtPlayerDiamantCard.Text = player.Ressources[4].ToString();
            //Gold coin
            //lxtPlayerDiamantCard.Text = player.Ressources[4].ToString();


            lblPlayer.Text = "Jeu de " + name;

            cmdPlay.Enabled = false;
        }

        /// <summary>
        /// click on the red coin (rubis) to tell the player has selected this coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblRubisCoin_Click(object sender, EventArgs e)
        {
            if (enableClicLabel)
            {
                int var = Convert.ToInt32(lblRubisCoin.Text);
                cmdValidateChoice.Visible = true;
                lblChoiceRubis.Visible = true;

                if (var == 2 && nbRubis == 1 )
                {
                    MessageBox.Show("Vous pouvez prendre 2 jetons de la même couleur uniquement à condition qu'il en reste au moin 4 dans la pile");
                }

                if (nbRubis == 2 || nbSaphir == 2 || nbOnyx == 2 || nbEmeraude == 2 || nbDiamand == 2)
                {
                    MessageBox.Show("Vous ne pouvez pas prendre ce jeton car vous avez déja pris 2 mêmes pierres précieuse");
                }
                else
                {
                    if ((nbRubis == 1 && nbSaphir == 1) || (nbRubis == 1 && nbOnyx == 1) || (nbRubis == 1 && nbEmeraude == 1) || (nbRubis == 1 && nbDiamand == 1))
                    {
                        MessageBox.Show("Vous ne pouvez par prendre 2 même jetons ainsi qu'un jeton différent");
                    }
                    else
                    {
                        nbTotal = nbRubis + nbSaphir + nbOnyx + nbEmeraude + nbDiamand;
                        if (nbTotal >= 3)
                        {
                            MessageBox.Show("Vous avez pris le nombre de jetons maximum");
                        }
                        else
                        {
                            nbRubis++;
                            var--;
                            lblRubisCoin.Text = var.ToString();
                            lblChoiceRubis.Text = nbRubis + "\r\n";
                        }
                    }
                }
            }  
                //TO DO check if possible to choose a coin, update the number of available coin  
            
        }


        /// <summary>
        /// click on the blue coin (saphir) to tell the player has selected this coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSaphirCoin_Click(object sender, EventArgs e)
        {
            if (enableClicLabel)
            {
                int var = Convert.ToInt32(lblSaphirCoin.Text);
                cmdValidateChoice.Visible = true;
                lblChoiceSaphir.Visible = true;

                if (var < 4)
                {
                    if (nbSaphir == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre 2 de ces jetons car il en reste moin de 4");
                    }
                }

                if (nbSaphir == 2 || nbRubis == 2 || nbOnyx == 2 || nbEmeraude == 2 || nbDiamand == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre ce jeton car vous en avez pris 2 d'une autre pierre précieuse");
                    }
                    else
                    {
                        if ((nbSaphir == 1 && nbRubis == 1) || (nbSaphir == 1 && nbOnyx == 1) || (nbSaphir == 1 && nbEmeraude == 1) || (nbSaphir == 1 && nbDiamand == 1))
                        {
                            MessageBox.Show("Vous ne pouvez pas faire ceci");
                        }
                        else
                        {
                            nbTotal = nbRubis + nbSaphir + nbOnyx + nbEmeraude + nbDiamand;
                            if (nbTotal >= 3)
                            {
                                MessageBox.Show("Vous avez pris le nombre de jetons maximum");
                            }
                            else
                            {
                                nbSaphir++;
                                var--; 
                                lblSaphirCoin.Text = var.ToString();
                                lblChoiceSaphir.Text = nbSaphir + "\r\n";
                            }

                        }
                    }
                
                //TO DO check if possible to choose a coin, update the number of available coin

            }
        }

        /// <summary>
        /// click on the black coin (onyx) to tell the player has selected this coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblOnyxCoin_Click(object sender, EventArgs e)
        {
            if (enableClicLabel)
            {
                int var = Convert.ToInt32(lblOnyxCoin.Text);
                cmdValidateChoice.Visible = true;
                lblChoiceOnyx.Visible = true;

                if (var < 4)
                {
                    if (nbOnyx == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre 2 de ces jetons car il en reste moin de 4");
                    }
                }

                if (nbOnyx == 2 || nbRubis == 2 || nbSaphir == 2 || nbEmeraude == 2 || nbDiamand == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre ce jeton car vous en avez pris 2 d'une autre pierre précieuse");
                    }
                    else
                    {
                        if ((nbOnyx == 1 && nbRubis == 1) || (nbOnyx == 1 && nbSaphir == 1) || (nbOnyx == 1 && nbEmeraude == 1) || (nbOnyx == 1 && nbDiamand == 1))
                        {
                            MessageBox.Show("Vous ne pouvez pas faire ceci");
                        }
                        else
                        {
                            nbTotal = nbRubis + nbSaphir + nbOnyx + nbEmeraude + nbDiamand;
                            if (nbTotal >= 3)
                            {
                                MessageBox.Show("Vous avez pris le nombre de jetons maximum");
                            }
                            else
                            {
                                nbOnyx++;
                                var--;
                                lblOnyxCoin.Text = var.ToString();
                                lblChoiceOnyx.Text = nbOnyx + "\r\n";
                            }

                        }
                    }
                
                //TO DO check if possible to choose a coin, update the number of available coin

            }
        }

        /// <summary>
        /// click on the green coin (emeraude) to tell the player has selected this coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEmeraudeCoin_Click(object sender, EventArgs e)
        {
            if (enableClicLabel)
            {
                int var = Convert.ToInt32(lblEmeraudeCoin.Text);
                cmdValidateChoice.Visible = true;
                lblChoiceEmeraude.Visible = true;

                if (var < 4)
                {
                    if (nbEmeraude == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre 2 de ces jetons car il en reste moin de 4");
                    }
                }

                if (nbEmeraude == 2 || nbRubis == 2 || nbOnyx == 2 || nbSaphir == 2 || nbDiamand == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre ce jeton car vous en avez pris 2 d'une autre pierre précieuse");
                    }
                    else
                    {
                        if ((nbEmeraude == 1 && nbRubis == 1) || (nbEmeraude == 1 && nbOnyx == 1) || (nbEmeraude == 1 && nbSaphir == 1) || (nbEmeraude == 1 && nbDiamand == 1))
                        {
                            MessageBox.Show("Vous ne pouvez pas faire ceci");
                        }
                        else
                        {
                            nbTotal = nbRubis + nbSaphir + nbOnyx + nbEmeraude + nbDiamand;
                            if (nbTotal >= 3)
                            {
                                MessageBox.Show("Vous avez pris le nombre de jetons maximum");
                            }
                            else
                            {
                                nbEmeraude++;
                                var--;
                                lblEmeraudeCoin.Text = var.ToString();
                                lblChoiceEmeraude.Text = nbEmeraude + "\r\n";
                            }

                        }
                    }
                
                //TO DO check if possible to choose a coin, update the number of available coin

            }
        }

        /// <summary>
        /// click on the white coin (diamand) to tell the player has selected this coin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblDiamandCoin_Click(object sender, EventArgs e)
        {
            if (enableClicLabel)
            {
                int var = Convert.ToInt32(lblDiamandCoin.Text);
                cmdValidateChoice.Visible = true;
                lblChoiceDiamand.Visible = true;

                if (var < 4)
                {
                    if (nbDiamand == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre 2 de ces jetons car il en reste moin de 4");
                    }
                }

                if (nbDiamand == 2 || nbRubis == 2 || nbOnyx == 2 || nbEmeraude == 2 || nbSaphir == 2)
                    {
                        MessageBox.Show("Vous ne pouvez pas prendre ce jeton car vous en avez pris 2 d'une autre pierre précieuse");
                    }
                    else
                    {
                        if ((nbDiamand == 1 && nbRubis == 1) || (nbDiamand == 1 && nbOnyx == 1) || (nbDiamand == 1 && nbEmeraude == 1) || (nbDiamand == 1 && nbSaphir == 1))
                        {
                            MessageBox.Show("Vous ne pouvez pas faire ceci");
                        }
                        else
                        {
                            nbTotal = nbRubis + nbSaphir + nbOnyx + nbEmeraude + nbDiamand;
                            if (nbTotal >= 3)
                            {
                                MessageBox.Show("Vous avez pris le nombre de jetons maximum");
                            }
                            else
                            {
                                nbDiamand++;
                                var--;
                                lblDiamandCoin.Text = var.ToString();
                                lblChoiceDiamand.Text = nbDiamand + "\r\n";
                            }

        /// <summary>
        /// click on the validate button to approve the selection of coins or card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdValidateChoice_Click(object sender, EventArgs e)
        {
            cmdNextPlayer.Visible = true;
            //TO DO Check if card or coins are selected, impossible to do both at the same time
            cmdValidateChoice.Visible = false;
            cmdNextPlayer.Enabled = true;
            // disable card Level one
            txtLevel11.Enabled = false;
            txtLevel12.Enabled = false;
            txtLevel13.Enabled = false;
            txtLevel14.Enabled = false;
            // disable card Level two
            txtLevel21.Enabled = false;
            txtLevel22.Enabled = false;
            txtLevel23.Enabled = false;
            txtLevel24.Enabled = false;
            // disable card Level three
            txtLevel31.Enabled = false;
            txtLevel32.Enabled = false;
            txtLevel33.Enabled = false;
            txtLevel34.Enabled = false;
            // disable lblCoin
            lblRubisCoin.Enabled = false;
            lblSaphirCoin.Enabled = false;
            lblEmeraudeCoin.Enabled = false;
            lblOnyxCoin.Enabled = false;
            lblDiamandCoin.Enabled = false;

            if (lblChoiceRubis.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceRubis.Text);
                int var2 = Convert.ToInt32(lblPlayerRubisCoin.Text);
                int Result = var + var2;

                lblPlayerRubisCoin.Text = Result.ToString();
                lblChoiceRubis.Text = "";
                lblChoiceRubis.Visible = false;
            }

            if (lblChoiceSaphire.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceSaphire.Text);
                int var2 = Convert.ToInt32(lblPlayerSaphireCoin.Text);
                int Result = var + var2;

                lblPlayerSaphireCoin.Text = Result.ToString();
                lblChoiceSaphire.Text = "";
                lblChoiceSaphire.Visible = false;
            }

            if (lblChoiceEmeraude.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceEmeraude.Text);
                int var2 = Convert.ToInt32(lblPlayerEmeraudeCoin.Text);
                int Result = var + var2;

                lblPlayerEmeraudeCoin.Text = Result.ToString();
                lblChoiceEmeraude.Text = "";
                lblChoiceEmeraude.Visible = false;
            }

            if (lblChoiceOnyx.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceOnyx.Text);
                int var2 = Convert.ToInt32(lblPlayerOnyxCoin.Text);
                int Result = var + var2;

                lblPlayerOnyxCoin.Text = Result.ToString();
                lblChoiceOnyx.Text = "";
                lblChoiceOnyx.Visible = false;
            }
            
            cmdNextPlayer.Enabled = true;

            if (lblChoiceRubis.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceRubis.Text);
                int var2 = Convert.ToInt32(lblPlayerRubisCoin.Text);
                int Result = var + var2;

                lblPlayerRubisCoin.Text = Result.ToString();
                lblChoiceRubis.Text = "";
                lblChoiceRubis.Visible = false;
            }

            if (lblChoiceSaphir.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceSaphir.Text);
                int var2 = Convert.ToInt32(lblPlayerSaphirCoin.Text);
                int Result = var + var2;

                lblPlayerSaphirCoin.Text = Result.ToString();
                lblChoiceSaphir.Text = "";
                lblChoiceSaphir.Visible = false;
            }

            if (lblChoiceEmeraude.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceEmeraude.Text);
                int var2 = Convert.ToInt32(lblPlayerEmeraudeCoin.Text);
                int Result = var + var2;

                lblPlayerEmeraudeCoin.Text = Result.ToString();
                lblChoiceEmeraude.Text = "";
                lblChoiceEmeraude.Visible = false;
            }

            if (lblChoiceOnyx.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceOnyx.Text);
                int var2 = Convert.ToInt32(lblPlayerOnyxCoin.Text);
                int Result = var + var2;

                lblPlayerOnyxCoin.Text = Result.ToString();
                lblChoiceOnyx.Text = "";
                lblChoiceOnyx.Visible = false;
            }

            if (lblChoiceDiamand.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceDiamand.Text);
                int var2 = Convert.ToInt32(lblPlayerDiamandCoin.Text);
                int Result = var + var2;

                lblPlayerDiamandCoin.Text = Result.ToString();
                lblChoiceDiamand.Text = "";
                lblChoiceDiamand.Visible = false;
            }
        }

            if (lblChoiceDiamant.Visible == true)
            {
                int var = Convert.ToInt32(lblChoiceDiamant.Text);
                int var2 = Convert.ToInt32(lblPlayerDiamantCoin.Text);
                int Result = var + var2;

                lblPlayerDiamantCoin.Text = Result.ToString();
                lblChoiceDiamant.Text = "";
                lblChoiceDiamant.Visible = false;
            }
            cmdNextPlayer.Visible = true;
        }

        /// <summary>
        /// click on the insert button to insert player in the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdInsertPlayer_Click(object sender, EventArgs e)
        {

            form2 = new FormAddPlayer();
            form2.Show();

           // MessageBox.Show("A implémenter");
        }

        /// <summary>
        /// click on the next player to tell him it is his turn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdNextPlayer_Click(object sender, EventArgs e)
        {
            
            cmdValidateChoice.Visible = false;
            cmdValidateChoice.Enabled = true;
            // enable card Level one
            txtLevel11.Enabled = true;
            txtLevel12.Enabled = true;
            txtLevel13.Enabled = true;
            txtLevel14.Enabled = true;
            // enable card Level two
            txtLevel21.Enabled = true;
            txtLevel22.Enabled = true;
            txtLevel23.Enabled = true;
            txtLevel24.Enabled = true;
            // enable card Level three
            txtLevel31.Enabled = true;
            txtLevel32.Enabled = true;
            txtLevel33.Enabled = true;
            txtLevel34.Enabled = true;
            //enable card Noble except cards already purchased
            txtNoble1.Enabled = true;
            txtNoble2.Enabled = true;
            txtNoble3.Enabled = true;
            txtNoble4.Enabled = true;
            // disable lblCoin
            lblRubisCoin.Enabled = true;
            lblSaphirCoin.Enabled = true;
            lblEmeraudeCoin.Enabled = true;
            lblOnyxCoin.Enabled = true;
            lblDiamandCoin.Enabled = true;

            
            //disable card already purchases
            for (int i = 0; i < NumCardAll.Count()-1; i++)
            {
                NameControlAll = NumCardAll[i];
                DisableControler.Name = NameControlAll;
                DisableControler.Enabled = false;
            }


            Coin[id, 0] = Convert.ToInt16(lblPlayerRubisCoin.Text);
            Coin[id, 1] = Convert.ToInt16(lblPlayerSaphireCoin.Text);
            Coin[id, 2] = Convert.ToInt16(lblPlayerOnyxCoin.Text);
            Coin[id, 3] = Convert.ToInt16(lblPlayerEmeraudeCoin.Text);
            Coin[id, 4] = Convert.ToInt16(lblPlayerDiamantCoin.Text);
            //Coin[id, 5] = Convert.ToInt16(lblPlayerGoldCoin.Text);

            Ressource[id, 0] = Convert.ToInt16(txtPlayerRubisCard.Text);
            Ressource[id, 1] = Convert.ToInt16(txtPlayerSaphireCard.Text);
            Ressource[id, 2] = Convert.ToInt16(txtPlayerOnyxCard.Text);
            Ressource[id, 3] = Convert.ToInt16(txtPlayerEmeraudeCard.Text);
            Ressource[id, 4] = Convert.ToInt16(txtPlayerDiamantCard.Text);




            if (id+1 < conn.NumberPlayer)
            {
                if (id+1 != conn.NumberPlayer && Reid == true)
                {
                    id++;
                }
                Reid = true;
                cmdNextPlayer.Visible = false;
                LoadPlayer(id);
                
            }
            else if(id == conn.NumberPlayer-1)
            {
                Reid = true;
                id = -1;
                id++;
                cmdNextPlayer.Visible = false;
                LoadPlayer(id);

            }
            



            //TO DO in release 1.0 : 3 is hard coded (number of players for the game), it shouldn't. 
            //TO DO Get the id of the player : in release 0.1 there are only 3 players
            //Reload the data of the player
            //We are not allowed to click on the next button
            
        }

        private void lblChoiceRubis_Click(object sender, EventArgs e)
        {
            int var = Convert.ToInt32(lblChoiceRubis.Text) - 1;
            int var2 = Convert.ToInt32(lblRubisCoin.Text) + 1;
            if (var >= 0 )
            {
                nbRubis--;
                lblChoiceRubis.Text = var.ToString();
                lblRubisCoin.Text = var2.ToString();

                if (var == 0)
                {
                    lblChoiceRubis.Visible = false;
                }
            }
        }

        private void lblChoiceSaphir_Click(object sender, EventArgs e)
        {
            int var = Convert.ToInt32(lblChoiceSaphir.Text) - 1;
            int var2 = Convert.ToInt32(lblSaphirCoin.Text) + 1;
            if (var >= 0)
            {
                nbSaphir--;
                lblChoiceSaphir.Text = var.ToString();
                lblSaphirCoin.Text = var2.ToString();

                if (var == 0)
                {
                    lblChoiceSaphir.Visible = false;
                }
            }
        }

        private void lblChoiceOnyx_Click(object sender, EventArgs e)
        {
            int var = Convert.ToInt32(lblChoiceOnyx.Text) - 1;
            int var2 = Convert.ToInt32(lblOnyxCoin.Text) + 1;
            if (var >= 0)
            {
                nbOnyx--;
                lblChoiceOnyx.Text = var.ToString();
                lblOnyxCoin.Text = var2.ToString();

                if (var == 0)
                {
                    lblChoiceOnyx.Visible = false;
                }
            }
        }

        private void lblChoiceEmeraude_Click(object sender, EventArgs e)
        {
            int var = Convert.ToInt32(lblChoiceEmeraude.Text) - 1;
            int var2 = Convert.ToInt32(lblEmeraudeCoin.Text) + 1;
            if (var >= 0)
            {
                nbEmeraude--;
                lblChoiceEmeraude.Text = var.ToString();
                lblEmeraudeCoin.Text = var2.ToString();

                if (var == 0)
                {
                    lblChoiceEmeraude.Visible = false;
                }
            }
        }

        private void lblChoiceDiamand_Click(object sender, EventArgs e)
        {
            int var = Convert.ToInt32(lblChoiceDiamand.Text) - 1;
            int var2 = Convert.ToInt32(lblDiamandCoin.Text) + 1;
            if (var >= 0)
            {
                nbDiamand--;
                lblChoiceDiamand.Text = var.ToString();
                lblDiamandCoin.Text = var2.ToString();

                if (var == 0)
                {
                    lblChoiceDiamand.Visible = false;
                }
            }
        }
    }
}
