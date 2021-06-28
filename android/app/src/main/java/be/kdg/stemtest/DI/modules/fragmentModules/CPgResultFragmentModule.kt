package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.custompartygame.CPgResult
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
internal abstract class CPgResultFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindCPgResultFragment(): CPgResult
}