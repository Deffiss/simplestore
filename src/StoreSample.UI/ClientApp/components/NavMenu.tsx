import * as React from 'react';
import { NavLink, Link, withRouter } from 'react-router-dom';
import * as H from 'history';

import { ICategoryAggregate } from '../models/ICategoryAggregate';
import { CategoryAggregate } from './CategoryAggregate';
import CatalogService from '../services/CatalogService';

interface State {
    categoryAggregates: ICategoryAggregate[],
}

class NavMenu extends React.Component<{ history: H.History, location: H.Location }, State> {
    constructor(props: any) {
        super(props);
        this.state = {
            categoryAggregates: [],
        }
    }

    public componentWillReceiveProps(nextProp: { history: H.History, location: H.Location }) {
        if (location.pathname != this.props.location.pathname && location.pathname=='/') {
            CatalogService.getCategoryAggregates().then(aggrs => {
                this.setState({ categoryAggregates: aggrs });
                if (aggrs.length > 0) {
                    this.props.history.push(`/${aggrs[0].id}`);
                }
            })
        }
    }

    public componentWillMount() {
        CatalogService.getCategoryAggregates().then(aggrs => {
            this.setState({ categoryAggregates: aggrs });
            if (aggrs.length > 0) {
                this.props.history.push(`/${aggrs[0].id}`);
            }
        })
    }
    public render() {
        return <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                      {
                          this.state.categoryAggregates.map(ca => {
                              return <CategoryAggregate key={ca.id} categoryAggregate={ca} />
                          })
                      }
                    </ul>
                </div>
            </div>
        </div>;
    }
}

export default withRouter(NavMenu);