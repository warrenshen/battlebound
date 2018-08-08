import React from 'react';
import PropTypes from 'prop-types';
import Link from 'gatsby-link';
import Helmet from 'react-helmet';

import {StyleRoot} from 'radium';

import 'animate.css/animate.css';
import 'bootstrap/dist/css/bootstrap.css';

import './index.css';
import './custom.css';

const TemplateWrapper = ({ children }) =>
  <StyleRoot>
    <div>
      <Helmet
        title="Battlebound"
        meta={[
          { name: 'description', content: 'Battlebound Blockchain Trading Card Game' },
          { name: 'keywords', content: 'battlebound, blockchain, ethereum, trading, card, game' },
          { name: 'viewport', content: 'width=device-width, initial-scale=1, minimum-scale=1' },
          { name: 'google-site-verification', content: '' },
          { property: 'og:image', content: 'https://etcetera.design/feature-photo.jpg' }
        ]}
      />
      <div>
        {children()}
      </div>
    </div>
  </StyleRoot>

TemplateWrapper.propTypes = {
  children: PropTypes.func,
}

export default TemplateWrapper;
