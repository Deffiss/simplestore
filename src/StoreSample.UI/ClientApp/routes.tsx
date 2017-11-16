import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Products } from './components/Products';

export const routes = <Layout>
    <Route exact path='/:id?' component={ Products } />
</Layout>;
