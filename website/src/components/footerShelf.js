import React from 'react';
import Link from 'gatsby-link';

import sharedStyles from '../shared/sharedStyles';
import colours from '../shared/colours';

import discord from '../assets/footer-icons/discord-icon.png';
import twitter from '../assets/footer-icons/twitter-icon.png';

import instagram from '../assets/footer-icons/instagram-icon.png';
import pinterest from '../assets/footer-icons/pinterest-icon.png';

import Circle from './decorations/circle';

class FooterShelf extends React.Component{

  render(){
    return (
      // <section className="diagonal clockwise" style={styles.section}>
      <section className="clockwise" style={styles.section}>
        <div style={styles.footer}>
          <ul style={styles.ul}>
            <li style={styles.li}><a style={styles.a} href=""><img src={discord} /></a></li>
            <li style={styles.li}><a style={styles.a} href="https://twitter.com/playbattlebound"><img src={twitter} /></a></li>
          </ul>
          <p style={styles.p}>Copyright 2018 &copy; Battlebound Inc</p>
        </div>
      </section>
    );
  }

}

var styles = {};

styles.section = {
  background: '#ffffff',
  paddingBottom: 0
}

styles.footer = {
  background: colours.backgroundGrey,
  padding: "30px 0px"
}

styles.ul = {
  listStyle: "none",
  textAlign: "center",
  marginLeft: "0"
}

styles.li = {
  display: "inline-block",
  width: "30px",
  margin: "0px 6px",
}

styles.a = {
  width: "100%"
}

styles.p = {
  fontFamily: "RobotoMono-Regular",
  fontSize: "18px",
  textAlign: "center",
  marginBottom: "10px"
}

styles.pSmall = Object.assign({}, styles.p, {
  fontSize: "14px"
})

export default FooterShelf;
