package be.kdg.stemtest.DI.modules

import be.kdg.stemtest.DI.modules.fragmentModules.*
import be.kdg.stemtest.DI.modules.fragmentModules.ConnectFragmentModule
import be.kdg.stemtest.DI.modules.fragmentModules.PartyOptionFragmentModule
import be.kdg.stemtest.view.MainActivity
import dagger.Subcomponent
import dagger.android.AndroidInjector

@Subcomponent(modules = [ConnectFragmentModule::class,
                        PartyOptionFragmentModule::class,
                        StatementFragmentModule::class,
                        ResultaatFragmentModule::class,
                        DgResultFragmentModule::class,
                        CDgResultFragmentModule::class,
                        CPgResultFragmentModule::class,
                        WachtschermFragmentModule::class,
                        MoreInfoFragmentModule::class,
                        PieFragmentModule::class])
 interface MainActivitySubcomponent : AndroidInjector<MainActivity> {
    @Subcomponent.Factory
     interface Factory :AndroidInjector.Factory<MainActivity>
}
