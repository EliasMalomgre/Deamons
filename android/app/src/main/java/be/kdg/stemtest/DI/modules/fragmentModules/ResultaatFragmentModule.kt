package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.partygame.PgResultaat
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class ResultaatFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindPartyFragment(): PgResultaat
}