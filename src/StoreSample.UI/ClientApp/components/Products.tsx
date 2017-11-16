import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { ICategory } from '../models/ICategory';
import { IProduct } from '../models/IProduct';
import CatalogService from '../services/CatalogService';
import { Product } from './Product';
import { ProductEditDialog } from './ProductEditDialog';

interface Params {
    id: string
}

interface State {
    products: IProduct[],
    category: ICategory,
    editDialogOpen: boolean,
}

export class Products extends React.Component<RouteComponentProps<Params>, State> {
    constructor(props: RouteComponentProps<Params>) {
        super(props);
        this.state = {
            products: [],
            editDialogOpen: false,
            category: {
                id: '',
                name: '',
                properties: {},
            }
        };
    }

    public setDialogState = (isopen: boolean) => {
        this.setState({ editDialogOpen: isopen });
    }

    public onSave = () => {
        const { history, location } = this.props;
        this.setDialogState(false);
        history.replace('/');
    }

    public render() {

        return <div>
            {this.state.editDialogOpen ? <ProductEditDialog onSave={this.onSave} /> : null}
            <div>
                <div className="row">
                    <div className="col-sm-9">
                        <h1 className="category__name">{this.state.category.name}</h1>
                    </div>
                    <div className="col-sm-3">
                        <button onClick={() => this.setDialogState(true)} className='btn btn-primary create-button'>Create product</button>
                    </div>
                </div>
                <hr/>
                <div className="clearfix"></div>
                <div className="products__container">
                    <div className="row">
                        {
                            this.state.products.map(p => {
                                return <div className="col-md-6 product__outer__wrapper" key={p.id}><Product key={p.id} product={p} /></div>
                            })
                        }
                    </div>
                </div>
            </div>
        </div>;
    }
    public componentWillMount() {
        const { match } = this.props;
        if (match && match.params.id) {
            CatalogService.getProducts(match.params.id).then(prods => {
                this.setState({ products: prods })
            })
            CatalogService.getCategory(match.params.id).then(category => {
                this.setState({ category })
            })
            
        }
    }

    public componentWillReceiveProps(newProps: RouteComponentProps<Params>) {
        const { match } = this.props;
        const { match: newMatch } = newProps;

        if (match == newMatch) {
            return;
        }

        let newId: string = null;
        let oldId: string = null;
        if (match) {
            oldId = match.params.id;
        }

        if (newMatch) {
            newId = newMatch.params.id;
        }

        if (oldId != newId && newId) {
            CatalogService.getProducts(newId).then(prods => {
                this.setState({ products: prods })
            })
            CatalogService.getCategory(newId).then(category => {
                this.setState({ category })
            })
        }
    }
}
