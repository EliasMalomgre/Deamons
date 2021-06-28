package be.kdg.stemtest.DI.modules

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import be.kdg.stemtest.DI.ViewModelFactory
import be.kdg.stemtest.viewmodel.PieViewModel
import be.kdg.stemtest.viewmodel.CDgResultViewModel
import be.kdg.stemtest.viewmodel.*
import be.kdg.stemtest.DI.ViewModelKey
import dagger.Binds
import dagger.Module
import dagger.multibindings.IntoMap


@Module
abstract class ViewModelModule {

    @Binds
    internal abstract fun bindViewModelFactory(factory: ViewModelFactory): ViewModelProvider.Factory

    @Binds
    @IntoMap
    @ViewModelKey(ConnectViewModel::class)
    internal abstract fun connectViewModel(viewModel: ConnectViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(PartyViewModel::class)
    internal abstract fun partijOptieViewModel(viewModel: PartyViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(ResultaatViewModel::class)
    internal abstract fun ResultaatViewModel(viewModel: ResultaatViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(DgStellingViewModel::class)
    internal abstract fun DgStellingViewModel(viewModel: DgStellingViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(DgResultViewModel::class)
    internal abstract fun DgResultViewModel(viewModel: DgResultViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(CDgResultViewModel::class)
    internal abstract fun CDgResultViewModel(viewModel: CDgResultViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(CPgResultViewModel::class)
    internal abstract fun CPgResultViewModel(viewModel: CPgResultViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(WachtschermViewModel::class)
    internal abstract fun WachtschermViewModel (viewModel: WachtschermViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(PgDgMoreInfoViewModel::class)
    internal abstract fun PgDgMoreInfoViewModel (viewModel: PgDgMoreInfoViewModel): ViewModel

    @Binds
    @IntoMap
    @ViewModelKey(PieViewModel::class)
    internal abstract fun PieViewModel (viewModel: PieViewModel): ViewModel

    //Add more ViewModels here
}